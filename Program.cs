using Microsoft.EntityFrameworkCore;

using TodoApi;

var builder = WebApplication.CreateBuilder(args);

// Add DI - AddServices
builder
    .Services
    .AddDbContext<TodoDb>(opt => opt.UseInMemoryDatabase("ToDoList"));

var app = builder.Build();

// Configure pipeline - Use Methods

app.MapGet("/todoitems", async (TodoDb db) =>
{
    var todos = await db.Todos.ToListAsync();

    return todos;
});

app.MapGet("/todoitems/{id}", async (int id, TodoDb db) =>
{
    var todos = await db.Todos.FindAsync(id);

    if (todos is null)
    {
        return Results.NotFound();
    }

    return Results.Ok(todos);
});

app.MapPost("/todoitems", async (TodoItem todo, TodoDb db) =>
{
    db.Todos.Add(todo);
    await db.SaveChangesAsync();
    return Results.Created($"/todoitems/{todo.Id}", todo);
});

app.MapPut("/todoitems/{id}", async (int id, TodoItem inputTodo, TodoDb db) =>
{
    var todo = await db.Todos.FindAsync(id);

    if (todo is null)
    {
        return Results.NotFound();
    }

    todo.Name = inputTodo.Name;
    todo.IsComplete = inputTodo.IsComplete;

    await db.SaveChangesAsync();
    return Results.NoContent();
});

app.MapDelete("/todoitems/{id}", async (int id, TodoDb db) =>
{
    if (await db.Todos.FindAsync(id) is TodoItem todo)
    {
        db.Todos.Remove(todo);
        await db.SaveChangesAsync();
        return Results.NoContent();
    }

    return Results.NotFound();
});

app.Run();
