using FluentValidation;

namespace MyGardenPatch.Commands;

public interface ICommandValidator<TCommand> : IValidator<TCommand> where TCommand : ICommand
{
}
