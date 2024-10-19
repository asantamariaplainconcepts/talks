using Microsoft.EntityFrameworkCore;

namespace aspire.ApiDbModel;

public class TodosDbContext(DbContextOptions<TodosDbContext> options) : DbContext(options)
{

    public DbSet<Todo> Todos { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Todo>().ToTable("todos");
        
        base.OnModelCreating(modelBuilder);
    }

    public static async Task SeedAsync(DbContext dbContext, bool storeManagementPerformed, CancellationToken cancellationToken)
    {
        var todosDbContext = (TodosDbContext)dbContext;

        if (!await todosDbContext.Todos.AnyAsync(cancellationToken))
        {
            todosDbContext.Todos.AddRange(
                new Todo { Title = "Mow the lawn" },
                new Todo { Title = "Take out the trash" },
                new Todo { Title = "Vacuum the house", CompletedOn = DateTime.UtcNow.AddDays(-4) },
                new Todo { Title = "Wash the car" },
                new Todo { Title = "Clean the gutters", CompletedOn = DateTime.UtcNow.AddDays(-7) }
            );

            await dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
