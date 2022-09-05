namespace MyGardenPatch.Gardeners.Commands;

public interface IGardenerCommand : ICommand
{
    GardenerId GardenerId { get; }
}
