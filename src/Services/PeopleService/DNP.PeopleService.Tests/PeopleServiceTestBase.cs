using DNP.ServiceProjectTestTemplate;
using Xunit.Abstractions;
using Xunit;
using DNP.PeopleService.Persistence;

namespace DNP.PeopleService.Tests;

[Collection(nameof(PersonalServiceTestCollection))]
public abstract class PeopleServiceTestBase<TTestFixture> :
    TestWithinContext<TTestFixture, PeopleDbContext> where TTestFixture : PeopleServiceTestFixture
{
    protected PeopleServiceTestBase(TTestFixture testFixture, ITestOutputHelper testOutputHelper)
        : base(testFixture, testOutputHelper)
    {
    }
}