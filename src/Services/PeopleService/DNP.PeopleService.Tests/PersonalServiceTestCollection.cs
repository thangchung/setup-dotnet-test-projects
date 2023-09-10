using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics;
using Testcontainers.MsSql;
using Xunit;

[assembly: CollectionBehavior(DisableTestParallelization = false)]

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
        Debug.WriteLine($"{nameof(PersonalServiceTestCollectionFixture)} constructor");

        this.Container = new MsSqlBuilder()
                .WithAutoRemove(true)
                .WithCleanUp(true)
                .WithHostname("test")
                .WithPortBinding(1433, assignRandomHostPort: true)
                .WithImage("mcr.microsoft.com/mssql/server:2022-latest")
                .WithPassword("P@ssw0rd-01")
                .WithStartupCallback(async (container, cancellationToken) =>
                {
                    Debug.WriteLine($"{nameof(PersonalServiceTestCollectionFixture)} - after container started");

                    var msSqlContainer = (MsSqlContainer)container;
                    this.Factory = new PeopleServiceWebApplicationFactory(msSqlContainer.GetConnectionString());
                    
                    await Task.Yield();
                })
                .Build();
    }

    public async Task DisposeAsync()
    {
        await this.Container.DisposeAsync();
        Debug.WriteLine($"{nameof(PersonalServiceTestCollectionFixture)} {nameof(DisposeAsync)}");
    }

    public async Task InitializeAsync()
    {
        Debug.WriteLine($"{nameof(PersonalServiceTestCollectionFixture)} {nameof(InitializeAsync)}");

        await this.Container.StartAsync();

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
