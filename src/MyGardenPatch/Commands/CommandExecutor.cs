using Microsoft.Extensions.DependencyInjection;

namespace MyGardenPatch.Commands
{
    public interface ICommandExecutor
    {
        Task HandleAsync<TCommand>(TCommand command, CancellationToken cancellationToken = default) where TCommand : ICommand;
    }

    public class CommandExecutor : ICommandExecutor
    {
        private readonly IServiceProvider _serviceProvider;

        public CommandExecutor(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task HandleAsync<TCommand>(TCommand command, CancellationToken cancellationToken = default) where TCommand : ICommand
        {
            var validator = _serviceProvider.GetRequiredService<ICommandValidator<TCommand>>() ?? throw new CommandValidatorNotFoundException<TCommand>();

            var result = await validator.ValidateAsync(command, cancellationToken);

            if (!result.IsValid)
                throw new InvalidCommandException<TCommand>(command, result.Errors);

            var handler = _serviceProvider.GetRequiredService<ICommandHandler<TCommand>>() ?? throw new CommandHandlerNotFoundException<TCommand>();

            await handler.HandleAsync(command, cancellationToken);
        }
    }
}
