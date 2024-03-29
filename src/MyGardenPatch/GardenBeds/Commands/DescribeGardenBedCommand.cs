﻿namespace MyGardenPatch.GardenBeds.Commands;

[Role(WellKnownRoles.Gardener)]
public record DescribeGardenBedCommand(
    GardenId GardenId, 
    GardenBedId GardenBedId, 
    string Name, 
    string Description, 
    Uri? ImageUri, 
    string? ImageDescription) : IGardenBedCommand,
                               INameableCommand,
                               IImageableCommand;

public class DescribeGardenBedCommandHandler : ICommandHandler<DescribeGardenBedCommand>
{
    private readonly IRepository<GardenBed, GardenBedId> _gardenBeds;

    public DescribeGardenBedCommandHandler(
        IRepository<GardenBed, GardenBedId> gardenBeds)
    {
        _gardenBeds = gardenBeds;
    }

    public async Task HandleAsync(
        DescribeGardenBedCommand command, 
        CancellationToken cancellationToken = default)
    {
        var gardenBed = await _gardenBeds.GetAsync(
            command.GardenBedId, 
            cancellationToken);

        gardenBed!.Describe(
            command.Name, 
            command.Description, 
            command.ImageUri, 
            command.ImageDescription);

        await _gardenBeds.AddOrUpdateAsync(gardenBed, cancellationToken);
    }
}

public class DescribeGardenBedCommandValidator : GardenBedCommandValidator<DescribeGardenBedCommand>, ICommandValidator<DescribeGardenBedCommand>
{
    public DescribeGardenBedCommandValidator(
        ICurrentUserProvider currentUser, 
        IRepository<Garden, GardenId> gardens, 
        IRepository<GardenBed, GardenBedId> gardenBeds) : base(currentUser, gardens, gardenBeds)
    {
        this.ValidateImageable();
        this.ValidateNameable();
    }
}
