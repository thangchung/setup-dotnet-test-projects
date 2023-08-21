using Xunit.Abstractions;
using Xunit;
using Bogus;

namespace DNP.PeopleService.Tests;

[Collection(nameof(PersonalServiceTestCollection))]
public abstract class PeopleServiceTestBase<TFixture>: IClassFixture<TFixture> where TFixture: PeopleServiceTestFixture
{
    protected readonly TFixture _fixture;
    protected readonly ITestOutputHelper _testOutput;
    protected readonly Faker _faker;

    protected PeopleServiceTestBase(TFixture fixture, ITestOutputHelper testOutput)
    {
        this._fixture = fixture;
        this._testOutput = testOutput;
        this._faker = new Faker();
    }

    protected Func<Faker, string> FakerPhoneNumber = faker => faker.Phone.PhoneNumber("## ### ####");

}