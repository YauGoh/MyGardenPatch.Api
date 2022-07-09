using MyGardenPatch.Events;
using MyGardenPatch.Gardens;

namespace MyGardenPatch.GardenBeds.DomainEvents;

internal record GardenBedRemoved(
    GardenId GardenId, 
    GardenBedId GardenBedId) : IDomainEvent;
