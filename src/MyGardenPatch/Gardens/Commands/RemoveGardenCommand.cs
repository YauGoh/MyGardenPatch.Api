using MyGardenPatch.Aggregates;
using MyGardenPatch.Commands;
using MyGardenPatch.Common;

namespace MyGardenPatch.Gardens.Commands
{
    public record RemoveGardenCommand(GardenId GardenId) : IGardenCommand;

    public class RemoveGardenCommandHandler : ICommandHandler<RemoveGardenCommand>
    {
        private readonly IRepository<Garden, GardenId> _gardens;

        public RemoveGardenCommandHandler(IRepository<Garden, GardenId> gardens)
        {
            _gardens = gardens;
        }

        public async Task HandleAsync(RemoveGardenCommand command, CancellationToken cancellationToken = default)
        {
            var garden = await _gardens.GetAsync(command.GardenId, cancellationToken);

            garden!.Remove();

            await _gardens.DeleteAsync(garden, cancellationToken);
        }
    }

    public class RemoveGardenCommandValidator : GardenCommandValidator<RemoveGardenCommand>, ICommandValidator<RemoveGardenCommand>
    {
        public RemoveGardenCommandValidator(ICurrentUserProvider currentUser, IRepository<Garden, GardenId> gardens) : base(currentUser, gardens)
        {
        }
    }
}
