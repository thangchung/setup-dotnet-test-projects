using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using System.Text.Json;
using Xunit;

[assembly: CollectionBehavior(DisableTestParallelization = true)]

namespace DNP.PeopleService.Tests;

[CollectionDefinition(nameof(PersonalServiceTestCollection))]
public class PersonalServiceTestCollection : ICollectionFixture<PeopleServiceWebApplicationFactory>
{

}

public class PeopleServiceWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder
            .UseEnvironment("Integration-Test")
            .UseContentRoot(Directory.GetCurrentDirectory())
            .UseTestServer()
            .ConfigureServices(services =>
            {
                services.RemoveAll<IHostedService>();
            })
            .ConfigureTestServices(this.ConfigureTestServices);
    }

    protected virtual void ConfigureTestServices(IServiceCollection services)
    {

    }

    public async Task ExecuteDbContextSaveChangeAsync(Func<DbContext, Task> func)
    {
        using var scope = this.Services.CreateAsyncScope();

        var dbContext = scope.ServiceProvider.GetRequiredService<DbContext>();

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
    }

    public async Task ExecuteDbContextQueryAsync(Func<DbContext, Task> func)
    {
        using var scope = this.Services.CreateAsyncScope();

        var dbContext = scope.ServiceProvider.GetRequiredService<DbContext>();

        await func.Invoke(dbContext);
    }

    public async Task ExecuteServiceAsync(Func<IServiceProvider, Task> func)
    {
        using var scope = this.Services.CreateAsyncScope();
        await func.Invoke(scope.ServiceProvider);
    }

    public JsonSerializerOptions JsonSerializerSettings
    {
        get
        {
            var jsonSettings = this.Services.GetRequiredService<IOptions<JsonOptions>>().Value;
            return jsonSettings?.SerializerOptions ?? new JsonSerializerOptions();
        }
    }

    public async Task ExecuteHttpClientAsync(Func<HttpClient, Task> func)
    {
        using var httpClient = this.CreateClient();
        await func.Invoke(httpClient);
    }
}
