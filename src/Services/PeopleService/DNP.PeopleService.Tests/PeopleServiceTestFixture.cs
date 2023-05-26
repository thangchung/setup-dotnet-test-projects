using DNP.PeopleService.Persistence;
using DNP.ServiceProjectTestTemplate;


namespace DNP.PeopleService.Tests;
public class PeopleServiceTestFixture : BaseTestFixture<PeopleDbContext>
{
    private readonly PeopleServiceWebApplicationFactory _webApplicationFactory;

    public PeopleServiceTestFixture(PeopleServiceWebApplicationFactory webApplicationFactory)
    {
        this._webApplicationFactory = webApplicationFactory;
    }

    protected override IServiceProvider ServiceProvider => this._webApplicationFactory.Services;

    public override HttpClient HttpClient => this._webApplicationFactory.CreateClient();
}
