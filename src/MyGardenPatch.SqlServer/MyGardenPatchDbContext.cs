using Microsoft.EntityFrameworkCore;
using MyGardenPatch.GardenBeds;
using MyGardenPatch.Gardens;
using MyGardenPatch.Users;
using System.Reflection;

namespace MyGardenPatch.SqlServer;

internal class MyGardenPatchDbContext : DbContext
{
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    public MyGardenPatchDbContext(DbContextOptions<MyGardenPatchDbContext> options) : base(options)
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    {
    }

    public DbSet<Garden> Gardens { get; set; }

    public DbSet<GardenBed> GardenBeds { get; set; }

    public DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}