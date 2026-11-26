using ServiceDefaults;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
var app = builder.Build();

app.MapDefaultEndpoints();

// In-memory todo store (for demonstration; replace with EF Core + Postgres in production)
var todos = new Dictionary<int, TodoItem>
{
    [1] = new TodoItem { Id = 1, Title = "Learn .NET Aspire", IsComplete = true },
    [2] = new TodoItem { Title = "Add Vue frontend", IsComplete = false },
    [3] = new TodoItem { Title = "Wire up API endpoints", IsComplete = false },
};
var nextId = 4;

app.MapGet("/", () => "Hello World!");

// GET /api/todos
app.MapGet("/api/todos", () => Results.Ok(todos.Values.OrderByDescending(t => t.Id)));

// GET /api/todos/{id}
app.MapGet("/api/todos/{id:int}", (int id) =>
    todos.TryGetValue(id, out var todo) ? Results.Ok(todo) : Results.NotFound());

// POST /api/todos
app.MapPost("/api/todos", (TodoItem todo) =>
{
    todo.Id = nextId++;
    todos[todo.Id] = todo;
    return Results.Created($"/api/todos/{todo.Id}", todo);
});

// PUT /api/todos/{id}
app.MapPut("/api/todos/{id:int}", (int id, TodoItem updated) =>
{
    if (!todos.ContainsKey(id)) return Results.NotFound();
    updated.Id = id;
    todos[id] = updated;
    return Results.NoContent();
});

// DELETE /api/todos/{id}
app.MapDelete("/api/todos/{id:int}", (int id) =>
{
    if (!todos.ContainsKey(id)) return Results.NotFound();
    todos.Remove(id);
    return Results.NoContent();
});

app.Run();

record TodoItem
{
    public int Id { get; set; }
    public required string Title { get; init; }
    public bool IsComplete { get; init; }
}
