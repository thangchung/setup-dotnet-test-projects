
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
    x.SetKebabCaseEndpointNameFormatter();

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(builder.Configuration.GetConnectionString("Rabbitmq")!);
        cfg.ConfigureEndpoints(context);
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting();

// app.MapGet("/", () => "Hello world");
app.MapGet("/", () => new TestModel(12, "hello"));

app.Run();

record TestModel(int Age, string Name);

public partial class Program { }