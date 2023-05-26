using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace DNP.ServiceProjectTestTemplate;

public abstract class BaseTestFixture<TDbContext> : IAsyncLifetime where TDbContext : DbContext
{
    public virtual Task DisposeAsync() => Task.CompletedTask;

    public virtual Task InitializeAsync() => Task.CompletedTask;

    protected abstract IServiceProvider ServiceProvider { get; }

    public abstract HttpClient HttpClient { get; }

    public async Task ExecuteDbContextAsync(Func<TDbContext, Task> func)
    {
        var serviceScopeFactory = this.ServiceProvider.GetRequiredService<IServiceScopeFactory>();

        using var scope = serviceScopeFactory.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<TDbContext>();

        await using var transaction = await dbContext.Database.BeginTransactionAsync();
        try
        {
            await func(dbContext);
            await transaction.CommitAsync();
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            throw;
        }

        await Task.Delay(200);
    }

    public async Task ExecuteServiceAsync(Func<IServiceProvider, Task> func)
    {
        var serviceScopeFactory = this.ServiceProvider.GetRequiredService<IServiceScopeFactory>();

        using var scope = serviceScopeFactory.CreateScope();

        await func(scope.ServiceProvider);
    }
}
