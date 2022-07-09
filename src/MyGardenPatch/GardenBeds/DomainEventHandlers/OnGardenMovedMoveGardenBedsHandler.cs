using MyGardenPatch.Aggregates;
using MyGardenPatch.Events;
using MyGardenPatch.Gardens.DomainEvents;

namespace MyGardenPatch.GardenBeds.DomainEventHandlers
{
    internal class OnGardenMovedMoveGardenBedsHandler : IDomainEventHandler<GardenMoved>
    {
        private readonly IRepository<GardenBed, GardenBedId> _gardenBeds;

        public OnGardenMovedMoveGardenBedsHandler(IRepository<GardenBed, GardenBedId> gardenBeds)
        {
            _gardenBeds = gardenBeds;
        }

        public async Task HandleAsync(GardenMoved domainEvent, CancellationToken cancellationToken)
        {
            if (!domainEvent.MoveGardenBeds) return;

            var affectedGardenBeds = await _gardenBeds.WhereAsync(gb => gb.GardenId == domainEvent.GardenId, cancellationToken);

            foreach (var gardenBed in affectedGardenBeds)
            {
                gardenBed.Move(domainEvent.Transformation);

                await _gardenBeds.AddOrUpdateAsync(gardenBed, cancellationToken);
            }
        }
    }
}
