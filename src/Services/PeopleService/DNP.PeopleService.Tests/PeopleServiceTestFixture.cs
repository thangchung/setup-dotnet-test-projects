using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json;
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

        await this.ExecuteDbContextAsync(async dbContext =>
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

    public async Task ExecuteServiceAsync(Func<IServiceProvider, Task> func)
    {
         await this._webApplicationFactory.ExecuteServiceAsync(func);
    }

    public async Task ExecuteTransactionDbContextAsync(Func<DbContext, Task> func)
    {
        await this._webApplicationFactory.ExecuteServiceAsync(async serviceProvider =>
        {
            var dbContext = serviceProvider.GetRequiredService<DbContext>();

            await using var transaction = await dbContext.Database.BeginTransactionAsync();
            try
            {
                await func.Invoke(dbContext);
                await transaction.CommitAsync();
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        });
    }

    public async Task ExecuteDbContextAsync(Func<DbContext, Task> func)
    {
        await this._webApplicationFactory.ExecuteServiceAsync(async serviceProvider =>
        {
            var dbContext = serviceProvider.GetRequiredService<DbContext>();

            await func.Invoke(dbContext);
        });
    }

    public async Task ExecuteHttpClientAsync(Func<HttpClient, Task> func)
    {
        using var httpClient = this._webApplicationFactory.CreateClient();
        await func.Invoke(httpClient);
    }

    public JsonSerializerOptions JsonSerializerOptions
        => this._webApplicationFactory.JsonSerializerSettings;

    public TModel? Parse<TModel>(string json) where TModel: class, new()
    {
        var model = default(TModel);

        if (string.IsNullOrEmpty(json)) return model;

        model = JsonSerializer.Deserialize<TModel>(json, this.JsonSerializerOptions);

        return model;
    }
}
