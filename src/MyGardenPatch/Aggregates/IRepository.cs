using MyGardenPatch.Events;
using System.Linq.Expressions;

namespace MyGardenPatch.Aggregates;

public interface IRepository<TAggregate, TKey>
    where TAggregate : Aggregate<TKey>
    where TKey : struct, IEquatable<TKey>, IEntityId
{
    Task<TAggregate?> GetAsync(TKey id, CancellationToken cancellationToken = default);

    Task<TAggregate?> GetByExpressionAsync(Expression<Func<TAggregate, bool>> expression, CancellationToken cancellationToken = default);

    Task<List<TAggregate>> WhereAsync(Expression<Func<TAggregate, bool>> expression, CancellationToken cancellationToken = default);

    Task<bool> AnyAsync(Expression<Func<TAggregate, bool>> expression, CancellationToken cancellationToken = default);

    Task AddOrUpdateAsync(TAggregate entity, CancellationToken cancellationToken = default);

    Task DeleteAsync(TAggregate entity, CancellationToken cancellationToken = default);
}

public abstract class AbstractRepository<TAggregate, TKey> : IRepository<TAggregate, TKey>
    where TAggregate : Aggregate<TKey>
    where TKey : struct, IEquatable<TKey>, IEntityId
{
    private readonly IDomainEventBus _bus;

    public AbstractRepository(IDomainEventBus bus)
    {
        _bus = bus;
    }

    public async Task AddOrUpdateAsync(TAggregate aggregate, CancellationToken cancellationToken = default)
    {
        await AddOrUpdateCoreAsync(aggregate, cancellationToken);

        await PublishEventsAsync(aggregate, cancellationToken);
    }

    public async Task DeleteAsync(TAggregate aggregate, CancellationToken cancellationToken = default)
    {
        await DeleteCoreAsync(aggregate, cancellationToken);

        await PublishEventsAsync(aggregate, cancellationToken);
    }

    public abstract Task<TAggregate?> GetAsync(TKey id, CancellationToken cancellationToken = default);

    public abstract Task<TAggregate?> GetByExpressionAsync(Expression<Func<TAggregate, bool>> expression, CancellationToken cancellationToken = default);

    public abstract Task<List<TAggregate>> WhereAsync(Expression<Func<TAggregate, bool>> expression, CancellationToken cancellationToken = default);

    public abstract Task<bool> AnyAsync(Expression<Func<TAggregate, bool>> expression, CancellationToken cancellationToken = default);

    protected abstract Task AddOrUpdateCoreAsync(TAggregate aggregate, CancellationToken cancellationToken);

    protected abstract Task DeleteCoreAsync(TAggregate aggregate, CancellationToken cancellationToken);

    private async Task PublishEventsAsync(TAggregate aggregagte, CancellationToken cancellationToken)
    {
        var events = aggregagte.GetRaisedDomainEvents();

        await Task.WhenAll(events.Select(e => (Task)PublishEventAsync((dynamic)e, cancellationToken)));
    }

    private Task PublishEventAsync<TEvent>(TEvent @event, CancellationToken cancellationToken) where TEvent : IDomainEvent
        => _bus.PublishAsync(@event, cancellationToken);
}
