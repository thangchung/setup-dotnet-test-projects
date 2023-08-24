using Microsoft.EntityFrameworkCore;
using Xunit;

namespace DNP.PeopleService.Tests;
public class PeopleServiceTestFixture : IAsyncLifetime
{
    private readonly PeopleServiceWebApplicationFactory _webApplicationFactory;

    public PeopleServiceTestFixture(PeopleServiceWebApplicationFactory webApplicationFactory)
    {
        this._webApplicationFactory = webApplicationFactory;
    }

    public async Task ExecuteAsync(Func<PeopleServiceWebApplicationFactory, Task> func)
    {
        await func.Invoke(this._webApplicationFactory);
    }

    public async virtual Task InitializeAsync()
    {
        await this._webApplicationFactory.StartContainerAsync();

        await this._webApplicationFactory.ExecuteDbContextQueryAsync(async dbContext =>
        {
            var database = dbContext.Database;
            var pendingMigrations = await database.GetPendingMigrationsAsync();
            if (pendingMigrations.Any())
            {
                await database.MigrateAsync();
            }
        });
    }

    public async virtual Task DisposeAsync() => await Task.Yield();
}
