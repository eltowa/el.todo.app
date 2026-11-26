using System.Threading.Tasks;
using Testcontainers.PostgreSql;
using Xunit;

namespace Todo.IntegrationTests.Fixtures;

/// <summary>
/// Fixture that starts a PostgreSQL container using Testcontainers.
/// Provides the connection string for tests that need a real database.
/// The container lifecycle is managed by xunit collection fixtures.
/// </summary>
public class PostgresContainerFixture : IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgres;

    public PostgresContainerFixture()
    {
        #pragma warning disable CS0618 // Type or member is obsolete (Testcontainers builder ctor)
        _postgres = new PostgreSqlBuilder()
            .WithImage("postgres:15-alpine")
            .WithDatabase("tododb")
            .WithUsername("postgres")
            .WithPassword("postgres")
            .WithPortBinding(5432, true)
            .Build();
        #pragma warning restore CS0618
    }

    /// <summary>
    /// The full Npgsql connection string for the running PostgreSQL container.
    /// </summary>
    public string ConnectionString => _postgres.GetConnectionString();

    /// <summary>
    /// The hostname of the PostgreSQL container.
    /// </summary>
    public string Hostname => _postgres.Hostname;

    /// <summary>
    /// The mapped PostgreSQL port on the host.
    /// </summary>
    public ushort Port => _postgres.GetMappedPublicPort(5432);

    public ValueTask InitializeAsync() => new(_postgres.StartAsync());

    public async ValueTask DisposeAsync()
    {
        await _postgres.StopAsync();
        GC.SuppressFinalize(this);
    }
}

/// <summary>
/// Collection definition that ensures PostgreSQL container is shared across all tests in this collection.
/// </summary>
[System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "xUnit1014:")]
public class PostgresContainerFixtureDefinition : ICollectionFixture<PostgresContainerFixture>
{
}
