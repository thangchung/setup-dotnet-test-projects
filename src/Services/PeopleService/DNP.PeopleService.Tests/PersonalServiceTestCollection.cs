using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics;
using Testcontainers.PostgreSql;
using Testcontainers.RabbitMq;
using Xunit;

[assembly: CollectionBehavior(DisableTestParallelization = false)]

namespace DNP.PeopleService.Tests;


[CollectionDefinition(nameof(PersonalServiceTestCollection))]
public class PersonalServiceTestCollection : ICollectionFixture<PersonalServiceTestCollectionFixture>
{

}

public class PersonalServiceTestCollectionFixture : IAsyncLifetime
{
    public PostgreSqlContainer PostgreSQLContainer { get; private set; }
    public RabbitMqContainer RabbitMQContainer { get; private set; }

    public PeopleServiceWebApplicationFactory Factory { get; private set; } = default!;

    private static readonly TaskFactory TaskFactory = new(CancellationToken.None, TaskCreationOptions.None, TaskContinuationOptions.None, TaskScheduler.Default);

    public PersonalServiceTestCollectionFixture()
    {
        Debug.WriteLine($"{nameof(PersonalServiceTestCollectionFixture)} constructor");

        TaskFactory.StartNew(LoadFactoryAsync)
          .Unwrap()
          .ConfigureAwait(false)
          .GetAwaiter()
          .GetResult();
    }

    public async Task LoadFactoryAsync()
    {
        Debug.WriteLine($"{nameof(PersonalServiceTestCollectionFixture)} - after container started");

        PostgreSQLContainer = new PostgreSqlBuilder()
                .WithAutoRemove(true)
                .WithCleanUp(true)
                .WithHostname("test")
                .WithPortBinding(5432, assignRandomHostPort: true)
                .WithDatabase("postgres")
                .WithUsername("postgres")
                .WithPassword("P@ssw0rd")
                .Build();

        RabbitMQContainer = new RabbitMqBuilder().Build();

        await PostgreSQLContainer.StartAsync().ConfigureAwait(false);
        await RabbitMQContainer.StartAsync().ConfigureAwait(false);

        Factory = new PeopleServiceWebApplicationFactory(
            PostgreSQLContainer.GetConnectionString(),
            RabbitMQContainer.GetConnectionString());
    }

    public async Task DisposeAsync()
    {
        await PostgreSQLContainer.DisposeAsync();
        await RabbitMQContainer.DisposeAsync();
        Debug.WriteLine($"{nameof(PersonalServiceTestCollectionFixture)} {nameof(DisposeAsync)}");
    }

    public async Task InitializeAsync()
    {
        Debug.WriteLine($"{nameof(PersonalServiceTestCollectionFixture)} {nameof(InitializeAsync)}");

        await PostgreSQLContainer.StartAsync();
        await RabbitMQContainer.StartAsync();

        await Factory.ExecuteServiceAsync(async serviceProvider =>
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
