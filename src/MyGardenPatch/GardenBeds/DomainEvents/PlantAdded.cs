using MyGardenPatch.Events;
using MyGardenPatch.Gardens;

namespace MyGardenPatch.GardenBeds.DomainEvents
{
    internal record PlantAdded(GardenId GardenId, GardenBedId GardenBedId, PlantId PlantId, DateTime PlantedAt) : IDomainEvent;
}
