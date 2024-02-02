﻿# Migration stuffs

## Generate

```bash
> dotnet ef migrations add InitPeopleDb -c PeopleDbContext -o Persistence/Migrations
or
> dotnet ef migrations update -c PeopleDbContext
```

```bash

dotnet ef migrations add Init_People_Schema `
-p src\Services\PeopleService\DNP.PeopleService `
-s src\Services\PeopleService\DNP.PeopleService `
-c PeopleDbContext `
-o Persistence\Migrations

```

```bash

dotnet ef database update `
-p src\Services\PeopleService\DNP.PeopleService `
-s src\Services\PeopleService\DNP.PeopleService `
-c PeopleDbContext

```
