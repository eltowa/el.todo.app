using System.Threading.Tasks;
using Testcontainers.Redis;
using Xunit;

namespace Todo.IntegrationTests.Fixtures;

/// <summary>
/// Fixture that starts a Redis container using Testcontainers.
/// Provides the connection string for tests that need a real cache.
/// The container lifecycle is managed by xunit collection fixtures.
/// </summary>
public class RedisContainerFixture : IAsyncLifetime
{
    private readonly RedisContainer _redis;

    public RedisContainerFixture()
    {
        #pragma warning disable CS0618 // Type or member is obsolete (Testcontainers builder ctor)
        _redis = new RedisBuilder()
            .WithImage("redis:7-alpine")
            .WithPortBinding(6379, true)
            .Build();
        #pragma warning restore CS0618
    }

    /// <summary>
    /// The full connection string for the running Redis container.
    /// </summary>
    public string ConnectionString => _redis.GetConnectionString();

    /// <summary>
    /// The hostname of the Redis container.
    /// </summary>
    public string Hostname => _redis.Hostname;

    /// <summary>
    /// The mapped Redis port on the host.
    /// </summary>
    public ushort Port => _redis.GetMappedPublicPort(6379);

    public ValueTask InitializeAsync() => new(_redis.StartAsync());

    public async ValueTask DisposeAsync()
    {
        await _redis.StopAsync();
        GC.SuppressFinalize(this);
    }
}

/// <summary>
/// Collection definition that ensures Redis container is shared across all tests in this collection.
/// </summary>
public class RedisContainerFixtureDefinition : ICollectionFixture<RedisContainerFixture>
{
}
