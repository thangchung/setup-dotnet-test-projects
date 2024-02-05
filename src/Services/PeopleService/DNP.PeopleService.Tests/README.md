# Get starting

[x] Try code coverage => https://github.com/microsoft/testfx/blob/main/samples/mstest-runner/Simple1/Simple1.csproj#L32 => was not worked, due to xUnit is not fully support for code coverage.
[x] Mock MassTransit with ITestHarness
[x] Mock identity context
[] Ignore some tests on EF Migrations folder

## Tools

```bash
> dotnet tool install dotnet-reportgenerator-globaltool
```

## Generate code coverage

```bash
> dotnet test --collect:"XPlat Code Coverage" /p:ExcludeByFile="../DNP.PeopleService/**/*Migrations/*.cs"
> # dotnet test --filter "FullyQualifiedName=DNP.PeopleService.Tests.TestCreatePerson" --collect:"XPlat Code Coverage"
```

```bash
> dotnet reportgenerator \
    -reports:"/home/thangchung/ip/spikes/setup-dotnet-test-projects/src/Services/PeopleService/DNP.PeopleService.Tests/TestResults/0e25bf88-40f0-4aec-9c04-d98393c55304/coverage.cobertura.xml" \
    -targetdir:"coveragereport" \
    -reporttypes:Html
```

## Refs
- https://devblogs.microsoft.com/dotnet/introducing-ms-test-runner/
- https://learn.microsoft.com/en-us/aspnet/core/test/integration-tests?view=aspnetcore-8.0
- https://learn.microsoft.com/en-us/dotnet/core/testing/unit-testing-code-coverage?tabs=windows#code-coverage-tooling
- https://learn.microsoft.com/en-us/dotnet/core/testing/unit-testing-code-coverage?tabs=windows#generate-reports
- https://dotnet.testcontainers.org/examples/aspnet/
- https://github.com/testcontainers/testcontainers-dotnet/blob/develop/examples/WeatherForecast/tests/WeatherForecast.InProcess.Tests/WeatherForecast.InProcess.Tests.csproj