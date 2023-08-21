
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

        await this.ExecuteAsync(async factory =>
        {
            await factory.ExecuteDbContextSaveChangeAsync(async dbContext =>
            {
                await dbContext.Set<Person>()
                    .Where(_ => this._personIds.Contains(_.Id))
                    .ExecuteDeleteAsync();

            });
        });
    }
}
