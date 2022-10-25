namespace MyGardenPatch.Gardens.Commands;

[Role(WellKnownRoles.Gardener)]
public record MoveGardenCommand(
    GardenId GardenId, 
    Point Point) : IGardenCommand, ILocateableCommand;

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

        garden!.Point = command.Point;

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
        this.ValidateLocatable();
    }
}
