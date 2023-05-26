# setup-dotnet-test-projects

An introduction how to setup and organize the test projects in .NET


## Overview

- Please read the [How to setup test project](/src/Services/PeopleService/DNP.PeopleService.Tests/README.md)

## Steps

### Starting up

```bash

docker compose -f docker-compose.infra.yaml up -d

```


```bash

dotnet ef database update `
-p src\Services\PeopleService\DNP.PeopleService `
-s src\Services\PeopleService\DNP.PeopleService `
-c PeopleDbContext

```


### Run the tests

1. The simple test has been defined at `DoTestCreatePerson`
2. The created data will be deleted after tested

### Cleanup

```bash

docker compose -f docker-compose.infra.yaml down -v

```