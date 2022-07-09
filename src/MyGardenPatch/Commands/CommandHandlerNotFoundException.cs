namespace MyGardenPatch.Commands;

internal class CommandHandlerNotFoundException<TCommand> : Exception where TCommand : ICommand
{
    public override string Message => $"No command handler found for {typeof(TCommand).FullName}";
}
