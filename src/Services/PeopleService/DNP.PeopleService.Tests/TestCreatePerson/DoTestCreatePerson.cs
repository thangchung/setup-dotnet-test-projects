using DNP.PeopleService.Domain;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;
using Xunit.Abstractions;

namespace DNP.PeopleService.Tests.TestCreatePerson;
public class DoTestCreatePerson : PeopleServiceTestBase
{
    public DoTestCreatePerson(PersonalServiceTestCollectionFixture testCollectionFixture, ITestOutputHelper testOutput) : base(testCollectionFixture, testOutput)
    {
    }

    protected readonly List<Guid> _personIds = new();

    private void DeletePerson(Guid? personId = null)
    {
        if (personId == null) return;
        this._personIds.Add(personId.Value);
    }

    public override async Task DisposeAsync()
    {
        await base.DisposeAsync();

        await this.ExecuteTransactionDbContextAsync(async dbContext =>
        {
            await dbContext.Set<Person>()
                    .Where(_ => this._personIds.Contains(_.Id))
                    .ExecuteDeleteAsync();
        });
    }

    [Fact]
    public async Task CreatePersonSuccessfully()
    {
        var person = new Person
        {
            Id = Guid.NewGuid(),
            Name = this._faker.Random.String2(40)
        };

        await this.ExecuteTransactionDbContextAsync(async dbContext =>
        {
            dbContext.Add(person);
            await dbContext.SaveChangesAsync();

            this.DeletePerson(person.Id);
        });

        await this.ExecuteDbContextAsync(async dbContext =>
        {
            person = await dbContext.Set<Person>().FirstOrDefaultAsync(p => p.Id == person.Id);
            person.Should().NotBeNull();
        });
    }
}
