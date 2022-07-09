using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyGardenPatch.GardenBeds;

namespace MyGardenPatch.SqlServer.EntityTypeConfigurations;

public class PlantEntityTypeConfiguration : IEntityTypeConfiguration<Plant>
{
    public void Configure(EntityTypeBuilder<Plant> builder)
    {
        builder
            .Property(e => e.Id)
            .HasConversion(id => id.Value, value => new PlantId(value));

        builder.HasName();

        builder.HasLocation();
    }
}