
using DNP.PeopleService.Domain;
using Microsoft.EntityFrameworkCore;

namespace DNP.PeopleService.Tests.TestCreatePerson;

public class CreatePersonTestFixture : PeopleServiceTestFixture
{
    public CreatePersonTestFixture(PeopleServiceWebApplicationFactory webApplicationFactory) : base(webApplicationFactory)
    {
    }

    protected readonly List<Guid> _personIds = new();

    public void DeletePerson(Guid? personId = null)
    {
        if (personId == null) return;
        this._personIds.Add(personId.Value);
    }

    public override async Task DisposeAsync()
    {
        await base.DisposeAsync();

        await this.ExecuteDbContextAsync(async dbContext =>
        {
            //var people = await dbContext.People.Where(p => this._personIds.Contains(p.Id)).ToListAsync();

            var people = this._personIds.Select(id => new Person { Id = id }).ToList();

            dbContext.RemoveRange(people);
            await dbContext.SaveChangesAsync();
        });
    }
}
