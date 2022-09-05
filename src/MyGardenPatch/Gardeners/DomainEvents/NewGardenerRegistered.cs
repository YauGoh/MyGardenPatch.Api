using MyGardenPatch.Events;

namespace MyGardenPatch.Gardeners.DomainEvents;

internal record class NewGardenerRegistered(GardenerId UserId, DateTime RegisteredAt) : IDomainEvent;