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

## Mock identity context

TODO