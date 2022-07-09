using FluentValidation;
using MyGardenPatch.Aggregates;
using MyGardenPatch.Commands;
using MyGardenPatch.Common;
using MyGardenPatch.Users.Exceptions;

namespace MyGardenPatch.Gardens.Commands
{
    public record StartNewGardenCommand(string Name, string Description, Location Location, Uri ImageUri, string ImageDescription, DateTime CreatedAt) : ICommand, INameableCommand, IImageableCommand;

    public class StartNewGardenCommandHandler : ICommandHandler<StartNewGardenCommand>
    {
        private readonly IRepository<Garden, GardenId> _gardens;
        private readonly ICurrentUserProvider _currentUserProvider;

        public StartNewGardenCommandHandler(IRepository<Garden, GardenId> gardens, ICurrentUserProvider currentUserProvider)
        {
            _gardens = gardens;
            _currentUserProvider = currentUserProvider;
        }

        public async Task HandleAsync(StartNewGardenCommand command, CancellationToken cancellationToken = default)
        {
            var garden = new Garden(
                _currentUserProvider.CurrentUserId ?? throw new UserNotAuthenticatedException(),
                command.Name,
                command.Description,
                command.ImageUri,
                command.ImageDescription,
                command.CreatedAt);

            garden.SetLocation(command.Location);

            await _gardens.AddOrUpdateAsync(garden, cancellationToken);
        }
    }

    public class StartNewGardenCommandValidator : AbstractValidator<StartNewGardenCommand>, ICommandValidator<StartNewGardenCommand>
    {
        public StartNewGardenCommandValidator()
        {
            this.ValidateNameable();
            this.ValidateImageable();
        }
    }
}
