namespace MyGardenPatch.SqlServer.EntityTypeConfigurations;

internal class UserEntityTypeConfiguration : IEntityTypeConfiguration<Gardener>
{
    public void Configure(EntityTypeBuilder<Gardener> builder)
    {
        builder
            .Property(e => e.Id)
            .HasConversion(id => id.Value, value => new GardenerId(value));

        builder.HasName();

        builder.Property(e => e.EmailAddress).HasMaxLength(200);

        builder
            .HasIndex(e => e.EmailAddress)
            .IsUnique();
    }
}
