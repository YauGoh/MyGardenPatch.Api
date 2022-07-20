namespace MyGardenPatch.LocalIdentity;

internal class LocalIdentityDbContext : IdentityDbContext<LocalIdentityUser, LocalIdentityRole, Guid>
{
    public LocalIdentityDbContext(DbContextOptions<LocalIdentityDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        builder.HasDefaultSchema("identity");
    }
}
