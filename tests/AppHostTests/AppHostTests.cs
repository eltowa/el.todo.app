using System.Linq;
using System.Threading;
using Aspire.Hosting.Testing;
using Xunit;

namespace Todo.AppHost.Tests;

public class AppHostTests
{
    [Fact]
    public async Task AppHostContainsPostgresServerResource()
    {
        var builder = await DistributedApplicationTestingBuilder.CreateAsync<Projects.AppHost>(TestContext.Current.CancellationToken);
        var postgres = builder.Resources.FirstOrDefault(r => r.Name == "postgres");
        Assert.NotNull(postgres);
    }

    [Fact]
    public async Task AppHostContainsPostgresDatabaseResource()
    {
        var builder = await DistributedApplicationTestingBuilder.CreateAsync<Projects.AppHost>(TestContext.Current.CancellationToken);
        var database = builder.Resources.FirstOrDefault(r => r.Name == "postgresdb");
        Assert.NotNull(database);
    }

    [Fact]
    public async Task AppHostContainsApiProjectResource()
    {
        var builder = await DistributedApplicationTestingBuilder.CreateAsync<Projects.AppHost>(TestContext.Current.CancellationToken);
        var api = builder.Resources.FirstOrDefault(r => r.Name == "api");
        Assert.NotNull(api);
    }

    [Fact]
    public async Task AppHostContainsMigrationProjectResource()
    {
        var builder = await DistributedApplicationTestingBuilder.CreateAsync<Projects.AppHost>(TestContext.Current.CancellationToken);
        var migration = builder.Resources.FirstOrDefault(r => r.Name == "migration");
        Assert.NotNull(migration);
    }

    [Fact]
    public async Task AppHostApiAndDatabaseResourcesExist()
    {
        var builder = await DistributedApplicationTestingBuilder.CreateAsync<Projects.AppHost>(TestContext.Current.CancellationToken);
        var api = builder.Resources.FirstOrDefault(r => r.Name == "api");
        var postgresDb = builder.Resources.FirstOrDefault(r => r.Name == "postgresdb");

        Assert.NotNull(api);
        Assert.NotNull(postgresDb);
    }
}
