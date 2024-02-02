# Get starting

## Generate code coverage

```bash
> dotnet test --collect:"XPlat Code Coverage"
```

```bash
> reportgenerator \
    -reports:"/home/thangchung/ip/spikes/setup-dotnet-test-projects/src/Services/PeopleService/DNP.PeopleService.Tests/TestResults/359a1a5a-68a2-4311-8eb8-52c807a32fa2/coverage.cobertura.xml" \
    -targetdir:"coveragereport" \
    -reporttypes:Html
```

Try code coverage => https://github.com/microsoft/testfx/blob/main/samples/mstest-runner/Simple1/Simple1.csproj#L32

## Mock identity context

TODO

## Refs
- https://devblogs.microsoft.com/dotnet/introducing-ms-test-runner/
- https://learn.microsoft.com/en-us/aspnet/core/test/integration-tests?view=aspnetcore-8.0
- https://learn.microsoft.com/en-us/dotnet/core/testing/unit-testing-code-coverage?tabs=windows#code-coverage-tooling
- https://dotnet.testcontainers.org/examples/aspnet/
- https://github.com/testcontainers/testcontainers-dotnet/blob/develop/examples/WeatherForecast/tests/WeatherForecast.InProcess.Tests/WeatherForecast.InProcess.Tests.csproj