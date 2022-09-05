using MyGardenPatch.Events;
using MyGardenPatch.Gardens;
using MyGardenPatch.Users;

namespace MyGardenPatch.GardenBeds.DomainEvents;

internal record GardenBedAdded(
    GardenerId GardenerId, 
    GardenId GardenId, 
    GardenBedId GardenBedId, 
    DateTime CreatedAt) : IDomainEvent;