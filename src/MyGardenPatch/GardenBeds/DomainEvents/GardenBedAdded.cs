using MyGardenPatch.Events;
using MyGardenPatch.Gardens;
using MyGardenPatch.Users;

namespace MyGardenPatch.GardenBeds.DomainEvents;

internal record GardenBedAdded(
    UserId UserId, 
    GardenId GardenId, 
    GardenBedId GardenBedId, 
    DateTime CreatedAt) : IDomainEvent;