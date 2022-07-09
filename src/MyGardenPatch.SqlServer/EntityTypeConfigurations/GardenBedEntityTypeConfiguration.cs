using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyGardenPatch.GardenBeds;
using MyGardenPatch.Gardens;
using MyGardenPatch.Users;

namespace MyGardenPatch.SqlServer.EntityTypeConfigurations;

internal class GardenBedEntityTypeConfiguration : IEntityTypeConfiguration<GardenBed>
{
    public void Configure(EntityTypeBuilder<GardenBed> builder)
    {
        builder
            .Property(e => e.Id)
            .HasConversion(id => id.Value, value => new GardenBedId(value));

        builder
            .Property(e => e.UserId)
            .HasConversion(id => id.Value, value => new UserId(value));
        builder.HasIndex(e => e.UserId);

        builder
            .Property(e => e.GardenId)
            .HasConversion(id => id.Value, value => new GardenId(value));
        builder.HasIndex(e => e.GardenId);

        builder.HasName();

        builder.HasLocation();

        builder.HasMany(e => e.Plants).WithOne().OnDelete(DeleteBehavior.Cascade);
    }
}
