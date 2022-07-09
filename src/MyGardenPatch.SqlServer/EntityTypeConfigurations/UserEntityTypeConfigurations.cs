using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyGardenPatch.Users;

namespace MyGardenPatch.SqlServer.EntityTypeConfigurations;

internal class UserEntityTypeConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder
            .Property(e => e.Id)
            .HasConversion(id => id.Value, value => new UserId(value));

        builder.HasName();

        builder.Property(e => e.EmailAddress).HasMaxLength(200);

        builder.HasIndex(e => e.EmailAddress);
    }
}
