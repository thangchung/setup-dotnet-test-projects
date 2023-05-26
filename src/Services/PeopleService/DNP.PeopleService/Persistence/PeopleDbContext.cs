using DNP.PeopleService.Domain;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace DNP.PeopleService.Persistence;

public class PeopleDbContext: DbContext
{
    public PeopleDbContext(DbContextOptions options) : base(options)
    {

    }

    public DbSet<Person> People => Set<Person>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}
