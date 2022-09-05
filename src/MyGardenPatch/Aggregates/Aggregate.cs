namespace MyGardenPatch.Aggregates;

public abstract class Aggregate<TKey> : Entity<TKey> where TKey : struct, IEquatable<TKey>, IEntityId
{
    private readonly List<IDomainEvent> _domainEvents = new List<IDomainEvent>();

    protected Aggregate(TKey id) : base(id)
    {

    }

    public IReadOnlyList<IDomainEvent> GetRaisedDomainEvents() => _domainEvents;

    public void ClearDomainEvents() => _domainEvents.Clear();

    protected void Raise<TDomainEvent>(TDomainEvent domainEvent) where TDomainEvent : IDomainEvent
        => _domainEvents.Add(domainEvent);
}

public abstract class GardenerOwnedAggregate<TKey> : Aggregate<TKey> where TKey : struct, IEquatable<TKey>, IEntityId
{
    protected GardenerOwnedAggregate(TKey id, GardenerId gardenerId) : base(id)
    {
        GardenerId = gardenerId;
    }

    protected GardenerOwnedAggregate(GardenerId userId) : this(new(), userId) { }

    public GardenerId GardenerId { get; private set; }
}
