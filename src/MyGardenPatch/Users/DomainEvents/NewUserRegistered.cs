using MyGardenPatch.Events;

namespace MyGardenPatch.Users.DomainEvents;

internal record class NewUserRegistered(UserId UserId, DateTime RegisteredAt) : IDomainEvent;