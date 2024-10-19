using aspire.ApiDbModel;
using Microsoft.EntityFrameworkCore;

namespace aspire.ApiService;

public static class TodosApi
{
    public static IEndpointRouteBuilder MapTodosApi(this IEndpointRouteBuilder app)
    {
        var prefix = "/todos";
        
        var todos = app.MapGroup(prefix);

        todos.MapGet("/", (TodosDbContext db) => db.Todos
            .OrderBy(x=>x.Id)
            .ToListAsync());

        todos.MapGet("/{id:int}", (int id, TodosDbContext db) => db.Todos.FirstOrDefaultAsync(t => t.Id == id));

        todos.MapPost("/", async (Todo todo, TodosDbContext db) =>
        {
            if (todo.Id != 0)
            {
                return Results.Problem($"{nameof(Todo.Id)} must not be specified when creating a new todo.",
                    statusCode: StatusCodes.Status400BadRequest);
            }

            if (todo.CreatedOn != default)
            {
                return Results.Problem(
                    $"{nameof(Todo.CreatedOn)} must not be specified when creating a new todo. This value is automatically set.",
                    statusCode: StatusCodes.Status400BadRequest);
            }

            if (!ApiValidator.IsValid(todo, out var validationErrors))
            {
                return Results.ValidationProblem(validationErrors);
            }

            db.Todos.Add(todo);

            await db.SaveChangesAsync();

            return Results.Created($"{prefix}/{todo.Id}", todo);
        });

        todos.MapPut("/{id:int}", async (int id, Todo todo, TodosDbContext db) =>
        {
            if (id != todo.Id)
            {
                return Results.Problem(
                    $"{nameof(Todo.Id)} in the path ({id}) does not match the {nameof(Todo.Id)} in the body ({todo.Id}).",
                    statusCode: StatusCodes.Status400BadRequest);
            }

            if (!ApiValidator.IsValid(todo, out var validationErrors))
            {
                return Results.ValidationProblem(validationErrors);
            }
            
            var todoToUpdate = await db.Todos.FindAsync(id);
            if (todoToUpdate == null)
            {
                return Results.NotFound();
            }
            
            todoToUpdate.Title = todo.Title;
            todoToUpdate.CompletedOn = todo.CompletedOn;
            
            await db.SaveChangesAsync();

            return Results.NoContent();

        });

        todos.MapDelete("/{id:int}", async (int id, TodosDbContext db) =>
        {
            var rowsDeleted = await db.Todos
                .Where(todo => todo.Id == id)
                .ExecuteDeleteAsync();

            return rowsDeleted == 1 ? Results.NoContent() : Results.NotFound();
        });

        return app;
    }
}