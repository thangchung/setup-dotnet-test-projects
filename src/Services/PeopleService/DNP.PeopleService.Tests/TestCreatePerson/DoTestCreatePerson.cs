using DNP.PeopleService.Domain;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;
using Xunit.Abstractions;

namespace DNP.PeopleService.Tests.TestCreatePerson;
public class DoTestCreatePerson : PeopleServiceTestBase<CreatePersonTestFixture>
{
    public DoTestCreatePerson(CreatePersonTestFixture testFixture, ITestOutputHelper testOutputHelper) 
            : base(testFixture, testOutputHelper)
    {
    }

    [Fact]
    public async Task CreatePersonSuccessfully()
    {
        var person = new Person
        {
            Id = Guid.NewGuid(),
            Name = this._faker.Random.String2(40)
        };

        await this._fixture.ExecuteTransactionDbContextAsync(async dbContext =>
        {
            dbContext.Add(person);
            await dbContext.SaveChangesAsync();

            this._fixture.DeletePerson(person.Id);
        });

        await this._fixture.ExecuteDbContextAsync(async dbContext =>
        {
            person = await dbContext.Set<Person>().FirstOrDefaultAsync(p => p.Id == person.Id);
            person.Should().NotBeNull();
        });
    }
}
