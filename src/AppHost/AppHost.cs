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
var api = builder.AddDotnetProject("api", "../Api/Api.csproj")
    .WithReference(postgresdb)
    .WithReference(redis)
    .WithReference(migration)
    .WaitFor(postgresdb);
    // TODO: Consider waiting for migration completion if necessary
    // .WaitForCompletion(migration);

// + JavaScript (Vue + Vite):
builder.AddViteApp("app", "../App")
    .WithPnpm()
    .WithRunScript("dev")
    .WithReference(api)
    .WaitFor(api)
    .WithExternalHttpEndpoints();

var app = builder.Build();

app.Run();
