using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyGardenPatch.Aggregates;
using MyGardenPatch.Common;

namespace MyGardenPatch.SqlServer.EntityTypeConfigurations;

internal static class EntityBuilderExtensions
{
    internal static PropertyBuilder<string> HasName<T>(this EntityTypeBuilder<T> builder) where T : class, INameable
        => builder.Property(e => e.Name).HasMaxLength(200);

    internal static PropertyBuilder<Location> HasLocation<T>(this EntityTypeBuilder<T> builder) where T : class, ILocateable
        => builder.Property(e => e.Location).JsonProperty(Location.Default);
}
