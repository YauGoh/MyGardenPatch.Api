using Microsoft.EntityFrameworkCore;
using MyGardenPatch.Aggregates;
using MyGardenPatch.Events;
using System.Linq.Expressions;

namespace MyGardenPatch.SqlServer;

internal class EntityFrameworkRepository<TAggregate, TKey> : AbstractRepository<TAggregate, TKey>
        where TAggregate : Aggregate<TKey>
        where TKey : struct, IEquatable<TKey>, IEntityId

{
    private readonly MyGardenPatchDbContext _dbContext;

    public EntityFrameworkRepository(IDomainEventBus bus, MyGardenPatchDbContext dbContext) : base(bus)
    {
        _dbContext = dbContext;
    }

    public override Task<TAggregate?> GetAsync(TKey id, CancellationToken cancellationToken = default)
    {
        Expression<Func<TAggregate, bool>> expression = e => e.Id.Equals(id);

        return GetByExpressionAsync(expression, cancellationToken);
    }

    public override Task<TAggregate?> GetByExpressionAsync(
        Expression<Func<TAggregate, bool>> expression,
        CancellationToken cancellationToken = default)
            => EntityFrameworkQueryableExtensions.FirstOrDefaultAsync(
                AggregateExtensions.AggregateSet(_dbContext, (dynamic)expression),
                cancellationToken);

    public override Task<List<TAggregate>> WhereAsync(Expression<Func<TAggregate, bool>> expression, CancellationToken cancellationToken = default)
        => EntityFrameworkQueryableExtensions.ToListAsync(
            AggregateExtensions.AggregateSet(_dbContext, (dynamic)expression),
            cancellationToken);

    public override Task<bool> AnyAsync(
        Expression<Func<TAggregate, bool>> expression,
        CancellationToken cancellationToken = default)
            => EntityFrameworkQueryableExtensions.AnyAsync(
                AggregateExtensions.AggregateSet(_dbContext, (dynamic)expression),
                cancellationToken);

    protected override async Task AddOrUpdateCoreAsync(TAggregate aggregate, CancellationToken cancellationToken)
    {
        var entityState = _dbContext.Entry(aggregate).State;

        if (entityState == EntityState.Detached)
            _dbContext.Add(aggregate);
        else
            _dbContext.Update(aggregate);

        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    protected override async Task DeleteCoreAsync(TAggregate aggregate, CancellationToken cancellationToken)
    {
        _dbContext.Remove(aggregate);

        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}