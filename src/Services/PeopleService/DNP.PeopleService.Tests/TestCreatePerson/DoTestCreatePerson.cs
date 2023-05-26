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

        await this.ExecuteDbContextAsync(async db =>
        {
            db.Add(person);
            await db.SaveChangesAsync();

            this._testFixture.DeletePerson(person.Id);
        });

        await this.ExecuteDbContextAsync(async db =>
        {
            person = await db.People.FirstOrDefaultAsync(p => p.Id == person.Id);
            person.Should().NotBeNull();
        });
    }
}
