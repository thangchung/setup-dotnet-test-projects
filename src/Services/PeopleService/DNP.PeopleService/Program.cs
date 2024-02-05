
using DNP.PeopleService.Consumers;
using DNP.PeopleService.Persistence;
using MassTransit;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var configuration = builder.Configuration;
var connectionString = configuration.GetConnectionString("Postgresql");
builder.Services.AddDbContext<DbContext, PeopleDbContext>(db =>
{
    db.UseNpgsql(connectionString, options =>
    {
        //options.EnableRetryOnFailure();
        options.MigrationsAssembly(typeof(PeopleDbContext).Assembly.GetName().Name);
    }).UseSnakeCaseNamingConvention();
});

builder.Services.AddMassTransit(x =>
{
    x.AddConsumersFromNamespaceContaining<SomethingDoneConsumer>();

    x.SetKebabCaseEndpointNameFormatter();

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(builder.Configuration.GetConnectionString("Rabbitmq")!);
        cfg.ConfigureEndpoints(context);
    });
});

builder.Services.AddAuthentication()
    .AddJwtBearer(options =>
    {
        options.Authority = "https://demo.duendesoftware.com";
        options.TokenValidationParameters.ValidateAudience = false;
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("ApiScope", policy =>
    {
        policy.RequireAuthenticatedUser();
        policy.RequireClaim("scope", "api");
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/", async (IPublishEndpoint endpoint) => {
    await endpoint.Publish(new SomethingDone("What's gone is gone!!!"));

    return new TestModel(12, "hello");
});

app.MapGet("/hello", () => new TestModel(12, "hello")).RequireAuthorization("ApiScope");

app.Run();

record TestModel(int Age, string Name);

public partial class Program { }