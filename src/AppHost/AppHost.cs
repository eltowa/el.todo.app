var builder = DistributedApplication.CreateBuilder(args);

// Infra:
// + PostgreSQL:
var postgres = builder.AddPostgres("postgres");
var postgresdb = postgres.AddDatabase("postgresdb");

// + Redis:
var redis = builder.AddRedis("redis");

// Projects:
// + Migration:
var migration = builder.AddDotnetProject(
    "migration",
    "../Migration/Migration.csproj")
    .WithReference(postgresdb)
    .WaitFor(postgresdb);

// + Api:
builder.AddDotnetProject("api", "../Api/Api.csproj")
    .WithReference(postgresdb)
    .WithReference(redis)
    .WithReference(migration)
    .WaitForCompletion(migration);

var app = builder.Build();

app.Run();
