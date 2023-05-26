using Bogus;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System.Text;
using System.Text.Json;
using Xunit.Abstractions;

namespace DNP.ServiceProjectTestTemplate;

public abstract class TestWithinContext<TTestFixture, TDbContext>: IClassFixture<TTestFixture> 
                where TTestFixture : BaseTestFixture<TDbContext>
                where TDbContext: DbContext
{
    protected readonly TTestFixture _testFixture;
    protected readonly ITestOutputHelper _testOutputHelper;
    protected readonly Faker _faker;

    protected TestWithinContext(TTestFixture testFixture, ITestOutputHelper testOutputHelper)
    {
        this._testFixture = testFixture;
        this._testOutputHelper = testOutputHelper;
        this._faker = new Faker();
    }

    protected async Task ExecuteDbContextAsync(Func<TDbContext, Task> func)
        => await _testFixture.ExecuteDbContextAsync(func);

    protected async Task ExecuteServiceAsync(Func<IServiceProvider, Task> func)
        => await _testFixture.ExecuteServiceAsync(func);

    protected HttpClient HttpClient => _testFixture.HttpClient;

    protected Func<Faker, string> FakerPhoneNumber = faker => faker.Phone.PhoneNumber("## ### ####");

    protected async Task ExecuteHttpRequest(object payload, 
                                            Func<HttpClient, StringContent, Task<HttpResponseMessage>> httpClient,
                                            Func<HttpResponseMessage, JsonSerializerOptions, Task> assert)
    {
        await this.ExecuteServiceAsync(async serviceProvider =>
        {
            var jsonOptions = serviceProvider.GetRequiredService<IOptions<JsonOptions>>().Value;
            var jsonSerializerOptions = jsonOptions.SerializerOptions;

            var payloadAsJson = JsonSerializer.Serialize(payload, jsonSerializerOptions);
            var payloadAsStringContext = new StringContent(payloadAsJson, Encoding.UTF8, "application/json");

            var response = await httpClient.Invoke(this.HttpClient, payloadAsStringContext);

            await assert.Invoke(response, jsonSerializerOptions);
        });
    }
}
