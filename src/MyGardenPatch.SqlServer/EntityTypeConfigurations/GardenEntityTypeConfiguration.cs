namespace MyGardenPatch.SqlServer.EntityTypeConfigurations;

public class GardenEntityTypeConfiguration : IEntityTypeConfiguration<Garden>
{
    public void Configure(EntityTypeBuilder<Garden> builder)
    {
        builder
            .Property(e => e.Id)
            .HasConversion(id => id.Value, value => new GardenId(value));

        builder
            .Property(e => e.GardenerId)
            .HasConversion(id => id.Value, value => new GardenerId(value));

        builder.HasIndex(e => e.GardenerId);

        builder.HasName();

        builder.HasLocation();

        builder.HasLocation();
    }
}