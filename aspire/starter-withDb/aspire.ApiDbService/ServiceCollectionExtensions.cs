using aspire.ApiDbModel;
using Microsoft.EntityFrameworkCore;

namespace aspire.ApiDbService;

internal static class ServiceCollectionExtensions
{
    public static IHostApplicationBuilder AddTodosDbContext(this IHostApplicationBuilder builder, string connectionName)
    {
        builder.Services.AddDbContextPool<TodosDbContext>(options =>
        {
            var connectionString = builder.Configuration.GetConnectionString(connectionName);

            options.UseNpgsql(connectionString,
                opt => opt.MigrationsAssembly(typeof(Program).Assembly.GetName()?.Name));
            
            options.UseSnakeCaseNamingConvention();

            options.UseAsyncSeeding(TodosDbContext.SeedAsync);
        });

        builder.EnrichNpgsqlDbContext<TodosDbContext>();

        builder.AddDbInitializer<TodosDbContext>();

        return builder;
    }

    private static IHostApplicationBuilder AddDbInitializer<TDbContext>(this IHostApplicationBuilder builder)
        where TDbContext : DbContext
    {
        builder.Services.AddSingleton<DbInitializer<TDbContext>>();

        builder.Services.AddHostedService(sp => sp.GetRequiredService<DbInitializer<TDbContext>>());

        builder.Services.AddHealthChecks()
            .AddCheck<DbInitializerHealthCheck<TDbContext>>($"{typeof(TDbContext).Name} initialization");

        return builder;
    }
}