using MyGardenPatch.Aggregates;
using MyGardenPatch.Commands;
using MyGardenPatch.Common;

namespace MyGardenPatch.Gardens.Commands;

public record DescribeGardenCommand(
    GardenId GardenId, 
    string Name, 
    string Description, 
    Uri ImageUri, 
    string ImageDescription) : IGardenCommand, INameableCommand, IImageableCommand;

public class DescribeGardenCommandHandler : ICommandHandler<DescribeGardenCommand>
{
    private readonly IRepository<Garden, GardenId> _gardens;

    public DescribeGardenCommandHandler(IRepository<Garden, GardenId> gardens)
    {
        _gardens = gardens;
    }

    public async Task HandleAsync(
        DescribeGardenCommand command, 
        CancellationToken cancellationToken = default)
    {
        var garden = await _gardens.GetAsync(command.GardenId);

        garden!.Describe(
            command.Name, 
            command.Description, 
            command.ImageUri, 
            command.ImageDescription);

        await _gardens.AddOrUpdateAsync(garden);
    }
}

public class DescribeGardenCommandValidator : GardenCommandValidator<DescribeGardenCommand>, ICommandValidator<DescribeGardenCommand>
{
    public DescribeGardenCommandValidator(ICurrentUserProvider currentUser, IRepository<Garden, GardenId> gardens) : base(currentUser, gardens)
    {
        this.ValidateNameable();
        this.ValidateImageable();
    }
}
