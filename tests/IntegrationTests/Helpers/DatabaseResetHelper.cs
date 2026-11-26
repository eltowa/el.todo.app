using System.Data.Common;
using System.Threading.Tasks;
using Npgsql;
using Respawn;

namespace Todo.IntegrationTests.Helpers;

/// <summary>
/// Helper that resets a database to a clean state using Respawn.
/// Use this between integration tests to avoid data leakage.
/// </summary>
public class DatabaseResetHelper : IAsyncDisposable
{
    private readonly Respawner _respawner;
    private readonly string _connectionString;
    private bool _disposed;

    public DatabaseResetHelper(string connectionString)
    {
        _connectionString = connectionString;
        var options = new RespawnerOptions
        {
            TablesToIgnore = ["__EFMigrationsHistory"],
            SchemasToInclude = ["public"]
        };

        // Respawn v7 requires a DbConnection, not a connection string
        var connection = CreateDbConnection(connectionString);
        _respawner = Respawner.CreateAsync(connection, options).GetAwaiter().GetResult();
    }

    /// <summary>
    /// Resets the database to a clean state by deleting all data from tracked tables.
    /// </summary>
    public Task ResetAsync() => _respawner.ResetAsync(CreateDbConnection(_connectionString));

    public async ValueTask DisposeAsync()
    {
        if (_disposed) return;
        _disposed = true;
        await ResetAsync();
    }

    private static DbConnection CreateDbConnection(string connectionString)
    {
        // Respawn works with any DbConnection; Npgsql is used for PostgreSQL
        return new NpgsqlConnection(connectionString);
    }
}
