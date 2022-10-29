using MyGardenPatch.Aggregates;
using MyGardenPatch.Common;

namespace MyGardenPatch.SqlServer.EntityTypeConfigurations;

internal static class EntityBuilderExtensions
{
    internal static PropertyBuilder<string> HasName<T>(this EntityTypeBuilder<T> builder) where T : class, INameable
        => builder.Property(e => e.Name).HasMaxLength(200);

    internal static PropertyBuilder<Point> HasLocation<T>(this EntityTypeBuilder<T> builder) where T : class, ILocateable
        => builder.Property(e => e.Center).JsonProperty(Point.Default);

    internal static PropertyBuilder<Shape?> HasShape<T>(this EntityTypeBuilder<T> builder) where T : class, IShapeable
        => builder.Property(e => e.Shape).JsonProperty<Shape?>(null);


}
