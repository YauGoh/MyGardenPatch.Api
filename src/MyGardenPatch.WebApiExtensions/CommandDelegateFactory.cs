namespace MyGardenPatch.WebApiExtensions
{
    internal static class CommandDelegateFactory
    {
        internal static Delegate GetDelegate<TCommand>() where TCommand : ICommand
            => async (TCommand command, ICommandExecutor commandExecutor, CancellationToken cancellationToken) =>
            {
                await commandExecutor.HandleAsync(command, cancellationToken);
            };
    }
}
