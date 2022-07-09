using FluentValidation;
using MyGardenPatch.Aggregates;
using MyGardenPatch.Commands;
using MyGardenPatch.Common;
using MyGardenPatch.Gardens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyGardenPatch.GardenBeds.Commands
{
    public record AddGardenBedCommand(GardenId GardenId, string Name, string Description, Location Location, Uri ImageUri, string ImageDescription, DateTime CreatedAt)
        : ICommand, INameableCommand, ILocateableCommand, IImageableCommand;

    public class AddGardenBedCommandHandler : ICommandHandler<AddGardenBedCommand>
    {
        private readonly IRepository<GardenBed, GardenBedId> _gardenBeds;
        private readonly ICurrentUserProvider _currentUserProvider;
        private readonly IDateTimeProvider _dateTime;

        public AddGardenBedCommandHandler(IRepository<GardenBed, GardenBedId> gardenBeds, ICurrentUserProvider currentUserProvider, IDateTimeProvider dateTime)
        {
            _gardenBeds = gardenBeds;
            _currentUserProvider = currentUserProvider;
            _dateTime = dateTime;
        }

        public async Task HandleAsync(AddGardenBedCommand command, CancellationToken cancellationToken = default)
        {
            var gardenBed = new GardenBed(
                _currentUserProvider.CurrentUserId!.Value,
                command.GardenId,
                command.Name,
                command.Description,
                command.ImageUri,
                command.ImageDescription,
                command.CreatedAt);

            gardenBed.SetLocation(command.Location);

            await _gardenBeds.AddOrUpdateAsync(gardenBed, cancellationToken);
        }
    }

    public class AddGardenBedCommandValidator : AbstractValidator<AddGardenBedCommand>, ICommandValidator<AddGardenBedCommand>
    {
        public AddGardenBedCommandValidator(IRepository<Garden, GardenId> gardens, ICurrentUserProvider currentUserProvider)
        {
            // Garden should exist and belong to current user
            RuleFor(c => c.GardenId)
                .MustAsync(async (gardenId, cancellationToken) => await gardens
                    .AnyAsync(
                        g => g.Id == gardenId &&
                             g.UserId == currentUserProvider.CurrentUserId,
                        cancellationToken))
                .WithMessage("Garden does not exist");

            this.ValidateNameable();

            this.ValidateLocatable();

            this.ValidateImageable();
        }
    }
}
