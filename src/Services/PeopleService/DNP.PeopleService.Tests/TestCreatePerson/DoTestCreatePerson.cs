using DNP.PeopleService.Consumers;
using DNP.PeopleService.Domain;
using FluentAssertions;
using MassTransit.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http.Headers;
using Xunit;
using Xunit.Abstractions;

namespace DNP.PeopleService.Tests.TestCreatePerson;

record TestModel(int Age, string Name);

public class DoTestCreatePerson(PersonalServiceTestCollectionFixture testCollectionFixture, ITestOutputHelper testOutput)
    : PeopleServiceTestBase(testCollectionFixture, testOutput)
{
    protected readonly List<Guid> _personIds = [];

    private void DeletePerson(Guid? personId = null)
    {
        if (personId == null) return;
        _personIds.Add(personId.Value);
    }

    public override async Task DisposeAsync()
    {
        await base.DisposeAsync();

        await ExecuteTransactionDbContextAsync(async dbContext =>
        {
            await dbContext.Set<Person>()
                    .Where(_ => _personIds.Contains(_.Id))
                    .ExecuteDeleteAsync();
        });
    }

    [Fact]
    public async Task CreatePersonSuccessfully()
    {
        var person = new Person
        {
            Id = Guid.NewGuid(),
            Name = _faker.Random.String2(40)
        };

        await ExecuteTransactionDbContextAsync(async dbContext =>
        {
            dbContext.Add(person);
            await dbContext.SaveChangesAsync();

            DeletePerson(person.Id);
        });

        await ExecuteDbContextAsync(async dbContext =>
        {
            person = await dbContext.Set<Person>().FirstOrDefaultAsync(p => p.Id == person.Id);
            person.Should().NotBeNull();
        });
    }

    [Fact]
    public async Task MassTransitTestHarnessNotNull()
    {
        await ExecuteServiceAsync(async (IServiceProvider provider) =>
        {
            var httpClient = _factory.CreateClient();

            httpClient.Should().NotBeNull();

            var testHarness = provider.GetRequiredService<ITestHarness>();
            testHarness.Should().NotBeNull();

            await testHarness.Start();

            var url = "/";
            var response = await httpClient.GetAsync(url);

            var result = await ParseResponse<TestModel>(response);

            var consumer = testHarness.GetConsumerHarness<SomethingDoneConsumer>();

            (await consumer.Consumed.Any<SomethingDone>(x => x.Context.Message.GreatMessage == "What's gone is gone!!!")).Should().BeTrue();
        });
    }

    [Fact]
    public async Task CanGetHomePage()
    {
        await ExecuteHttpClientAsync(async httpClient =>
        {
            var url = "/";
            var response = await httpClient.GetAsync(url);

            var result = await ParseResponse<TestModel>(response);

            var expectedResult = new TestModel(12, "hello");

            result.Age.Equals(12);
            result.Name.Equals("hello");
        });
    }

    [Fact]
    public async Task CanGetHelloPage_AuthnAuthz()
    {
        await ExecuteHttpClientAsync(async httpClient =>
        {
            var url = "/hello";

            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(scheme: "TestScheme");
            var response = await httpClient.GetAsync(url);

            var result = await ParseResponse<TestModel>(response);

            var expectedResult = new TestModel(12, "hello");

            result.Age.Equals(12);
            result.Name.Equals("hello");
        });
    }
}
