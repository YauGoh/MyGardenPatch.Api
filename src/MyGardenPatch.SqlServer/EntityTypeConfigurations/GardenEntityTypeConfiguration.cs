using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyGardenPatch.Gardens;
using MyGardenPatch.Users;

namespace MyGardenPatch.SqlServer.EntityTypeConfigurations;

public class GardenEntityTypeConfiguration : IEntityTypeConfiguration<Garden>
{
    public void Configure(EntityTypeBuilder<Garden> builder)
    {
        builder
            .Property(e => e.Id)
            .HasConversion(id => id.Value, value => new GardenId(value));

        builder
            .Property(e => e.UserId)
            .HasConversion(id => id.Value, value => new UserId(value));

        builder.HasIndex(e => e.UserId);

        builder.HasName();

        builder.HasLocation();
    }
}