namespace MyGardenPatch.Aggregates
{
    public interface IEntityId { }

    public abstract class Entity<TKey> where TKey : struct, IEquatable<TKey>, IEntityId
    {
        public Entity(TKey id)
        {
            Id = id;
        }

        public TKey Id { get; private set; } = new TKey();
    }
}
