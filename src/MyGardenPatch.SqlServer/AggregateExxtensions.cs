using Microsoft.EntityFrameworkCore;
using MyGardenPatch.GardenBeds;
using System.Linq.Expressions;

namespace MyGardenPatch.SqlServer;

internal static class AggregateExtensions
{
    internal static IQueryable<TAggregate> AggregateSet<TAggregate>(
        this MyGardenPatchDbContext dbContext,
        Expression<Func<TAggregate, bool>> where) where TAggregate : class
            => dbContext.Set<TAggregate>().Where(where);


    internal static IQueryable<GardenBed> AggregateSet(
        this MyGardenPatchDbContext dbContext,
        Expression<Func<GardenBed, bool>> where)
            => dbContext.GardenBeds
                .Include(gb => gb.Plants)
                .Where(where);
}