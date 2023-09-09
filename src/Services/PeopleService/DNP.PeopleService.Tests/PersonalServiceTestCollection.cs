using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Testcontainers.MsSql;
using Xunit;

[assembly: CollectionBehavior(DisableTestParallelization = true)]

namespace DNP.PeopleService.Tests;


[CollectionDefinition(nameof(PersonalServiceTestCollection))]
public class PersonalServiceTestCollection : ICollectionFixture<PersonalServiceTestCollectionFixture>
{

}

public class PersonalServiceTestCollectionFixture : IAsyncLifetime
{
    public MsSqlContainer Container { get; }

    public PeopleServiceWebApplicationFactory Factory { get; private set; } = default!;

    public PersonalServiceTestCollectionFixture()
    {
        this.Container = new MsSqlBuilder()
                .WithAutoRemove(true)
                .WithCleanUp(true)
                .WithHostname("test")
                .WithExposedPort(14333)
                .WithImage("mcr.microsoft.com/mssql/server:2022-latest")
                .WithPassword("P@ssw0rd-01")
                .Build();
    }

    public async Task DisposeAsync()
    {
        await this.Container.DisposeAsync();
    }

    public async Task InitializeAsync()
    {
        await this.Container.StartAsync();

        this.Factory = new PeopleServiceWebApplicationFactory(this.Container.GetConnectionString());

        await this.Factory.ExecuteServiceAsync(async serviceProvider =>
        {
            var dbContext = serviceProvider.GetRequiredService<DbContext>();

            var database = dbContext.Database;

            var pendingMigrations = await database.GetPendingMigrationsAsync();

            if (pendingMigrations.Any())
            {
                await database.MigrateAsync();
            }
        });
    }
}
