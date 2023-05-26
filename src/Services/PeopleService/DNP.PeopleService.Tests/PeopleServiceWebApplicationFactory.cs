using DNP.ServiceProjectTestTemplate;
using Xunit;

[assembly: CollectionBehavior(DisableTestParallelization = true)]

namespace DNP.PeopleService.Tests;

[CollectionDefinition(nameof(PersonalServiceTestCollection))]
public class PersonalServiceTestCollection : ICollectionFixture<PeopleServiceWebApplicationFactory>
{

}

public class PeopleServiceWebApplicationFactory : WebApplicationFactoryBase<Program>
{
}
