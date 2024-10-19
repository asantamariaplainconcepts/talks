using aspire.ApiDbModel;
using aspire.ApiService;
using aspire.ServiceDefaults;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddProblemDetails();

builder.AddNpgsqlDbContext<TodosDbContext>("todosdb",
    configureDbContextOptions: options => { options.UseSnakeCaseNamingConvention(); });

builder.EnrichNpgsqlDbContext<TodosDbContext>();

var app = builder.Build();

app.UseExceptionHandler();

app.MapTodosApi();

app.MapDefaultEndpoints();

app.Run();