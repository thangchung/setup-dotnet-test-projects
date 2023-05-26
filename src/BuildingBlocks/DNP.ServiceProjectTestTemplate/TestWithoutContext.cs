using Xunit.Abstractions;

namespace DNP.ServiceProjectTestTemplate;

public abstract class TestWithoutContext
{
    protected ITestOutputHelper _testOutputHelper;

    protected TestWithoutContext(ITestOutputHelper testOutputHelper)
        => this._testOutputHelper = testOutputHelper;
}
