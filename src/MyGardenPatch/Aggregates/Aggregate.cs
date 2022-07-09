using MyGardenPatch.Events;
using MyGardenPatch.Users;

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

public abstract class UserOwnedAggregate<TKey> : Aggregate<TKey> where TKey : struct, IEquatable<TKey>, IEntityId
{
    protected UserOwnedAggregate(TKey id, UserId userId) : base(id)
    {
        UserId = userId;
    }

    protected UserOwnedAggregate(UserId userId) : this(new(), userId) { }

    public UserId UserId { get; private set; }
}
