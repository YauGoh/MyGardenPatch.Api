using MyGardenPatch.Events;
using MyGardenPatch.Gardens;

namespace MyGardenPatch.GardenBeds.DomainEvents;

internal record PlantRemoved(
    GardenId GardenId, 
    GardenBedId GardenBedId, 
    PlantId PlantId) : IDomainEvent;
