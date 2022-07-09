using MyGardenPatch.Aggregates;
using MyGardenPatch.Commands;
using MyGardenPatch.Common;
using MyGardenPatch.Gardens;

namespace MyGardenPatch.GardenBeds.Commands
{
    public record MoveGardenBedCommand(
        GardenId GardenId, 
        GardenBedId GardenBedId, 
        Transformation Transformation)
       : IGardenBedCommand;

    public class MoveGardenBedCommandHandler : ICommandHandler<MoveGardenBedCommand>
    {
        private readonly IRepository<GardenBed, GardenBedId> _gardenBeds;

        public MoveGardenBedCommandHandler(
            IRepository<GardenBed, GardenBedId> gardenBeds)
        {
            _gardenBeds = gardenBeds;
        }

        public async Task HandleAsync(
            MoveGardenBedCommand command, 
            CancellationToken cancellationToken = default)
        {
            var gardenBed = await _gardenBeds.GetAsync(
                command.GardenBedId,
                cancellationToken);

            gardenBed!.Move(command.Transformation);

            await _gardenBeds.AddOrUpdateAsync(
                gardenBed,
                cancellationToken);
        }
    }

    public class MoveGardenBedCommandValidator : GardenBedCommandValidator<MoveGardenBedCommand>, ICommandValidator<MoveGardenBedCommand>
    {
        public MoveGardenBedCommandValidator(
            ICurrentUserProvider currentUser,
            IRepository<Garden, GardenId> gardens,
            IRepository<GardenBed, GardenBedId> gardenBeds)
            : base(currentUser, gardens, gardenBeds)
        {
        }
    }
}
