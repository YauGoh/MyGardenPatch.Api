using MyGardenPatch.Common;
using MyGardenPatch.Events;

namespace MyGardenPatch.Gardens.DomainEvents
{
    internal record GardenMoved(GardenId GardenId, Transformation Transformation, bool MoveGardenBeds) : IDomainEvent;
}
