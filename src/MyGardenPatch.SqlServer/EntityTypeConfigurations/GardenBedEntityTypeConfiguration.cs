

namespace MyGardenPatch.SqlServer.EntityTypeConfigurations;

internal class GardenBedEntityTypeConfiguration : IEntityTypeConfiguration<GardenBed>
{
    public void Configure(EntityTypeBuilder<GardenBed> builder)
    {
        builder
            .Property(e => e.Id)
            .HasConversion(id => id.Value, value => new GardenBedId(value));

        builder
            .Property(e => e.GardenerId)
            .HasConversion(id => id.Value, value => new GardenerId(value));
        builder.HasIndex(e => e.GardenerId);

        builder
            .Property(e => e.GardenId)
            .HasConversion(id => id.Value, value => new GardenId(value));
        builder.HasIndex(e => e.GardenId);

        builder.HasName();

        builder.HasLocation();

        builder.HasMany(e => e.Plants).WithOne().OnDelete(DeleteBehavior.Cascade);
    }
}
