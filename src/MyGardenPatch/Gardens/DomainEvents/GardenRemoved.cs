using MyGardenPatch.Events;

namespace MyGardenPatch.Gardens.DomainEvents
{
    internal record GardenRemoved(GardenId GardenId) : IDomainEvent;
}
