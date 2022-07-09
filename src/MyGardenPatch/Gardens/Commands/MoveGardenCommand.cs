using MyGardenPatch.Aggregates;
using MyGardenPatch.Commands;
using MyGardenPatch.Common;

namespace MyGardenPatch.Gardens.Commands;

public record MoveGardenCommand(
    GardenId GardenId, 
    Transformation Transformation, 
    bool MoveGardenBeds) : IGardenCommand;

public class MoveGardenCommandHandler : ICommandHandler<MoveGardenCommand>
{
    private readonly IRepository<Garden, GardenId> _gardens;

    public MoveGardenCommandHandler(IRepository<Garden, GardenId> gardens)
    {
        _gardens = gardens;
    }

    public async Task HandleAsync(
        MoveGardenCommand command, 
        CancellationToken cancellationToken = default)
    {
        var garden = await _gardens.GetAsync(
            command.GardenId, 
            cancellationToken);

        garden!.Move(
            command.Transformation, 
            command.MoveGardenBeds);

        await _gardens.AddOrUpdateAsync(
            garden, 
            cancellationToken);
    }
}

public class MoveGardenCommandValidator : GardenCommandValidator<MoveGardenCommand>
{
    public MoveGardenCommandValidator(
        ICurrentUserProvider currentUser, 
        IRepository<Garden, GardenId> gardens) : base(currentUser, gardens)
    {
    }
}
