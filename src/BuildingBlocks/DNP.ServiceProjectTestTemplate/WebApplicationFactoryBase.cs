using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;

namespace DNP.ServiceProjectTestTemplate;

public abstract class WebApplicationFactoryBase<TProgramStartup> : WebApplicationFactory<TProgramStartup> where TProgramStartup : class
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

                this.ConfigureTestServices(services);
            });
    }

    protected virtual void ConfigureTestServices(IServiceCollection services)
    {

    }
}
