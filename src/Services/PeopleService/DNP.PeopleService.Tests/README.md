# Setup test project

## Setting up

1. Reference to `DNP.ServiceProjectTestTemplate` & `DNP.PeopleService`
1. Define `PeopleServiceWebApplicationFactory` inherits `WebApplicationFactoryBase<Program>`. Also define the test-collection
1. Define `PeopleServiceTestFixture` inherits `BaseTestFixture<PeopleDbContext>`
1. Define `PeopleServiceTestBase<TTestFixture>`

## Define Tests

1. Define `CreatePersonTestFixture` inherits `PeopleServiceTestFixture`
2. Define `DoTestCreatePerson` inherits `PeopleServiceTestBase<CreatePersonTestFixture>`
