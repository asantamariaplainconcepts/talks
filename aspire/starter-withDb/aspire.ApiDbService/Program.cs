using aspire.ApiDbModel;
using aspire.ApiDbService;
using aspire.ServiceDefaults;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.AddTodosDbContext("todosdb");

builder.Services.AddProblemDetails();

var app = builder.Build();

app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.MapPost("/reset-db", async (TodosDbContext db) =>
    {
        // Delete and recreate the database. This is useful for development scenarios to reset the database to its initial state.
        await db.Database.EnsureDeletedAsync();
        await db.Database.MigrateAsync();
    });
}

app.MapDefaultEndpoints();

app.Run();
