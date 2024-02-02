using DNP.PeopleService.Domain;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
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
    public async Task CanGetHomePage()
    {
        await ExecuteHttpClientAsync(async httpClient => {
            var url = "/";
            var response = await httpClient.GetAsync(url);
            
            var result = await ParseResponse<TestModel>(response);

            var expectedResult = new TestModel(12, "hello");

            result.Age.Equals(12);
            result.Name.Equals("hello");
        });
    }
}
