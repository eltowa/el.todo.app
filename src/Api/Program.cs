using ServiceDefaults;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

var app = builder.Build();

app.MapDefaultEndpoints();

// Root health endpoint
app.MapGet("/", () => "Hello World!");

app.Run();
