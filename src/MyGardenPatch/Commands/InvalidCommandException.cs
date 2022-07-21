using FluentValidation.Results;

namespace MyGardenPatch.Commands;

public class InvalidCommandException<TCommand> : Exception
{
    public InvalidCommandException(TCommand command, IDictionary<string, string[]> errors)
    {
        Command = command;
        Errors = errors;
    }

    public TCommand Command { get; }

    public IDictionary<string, string[]> Errors { get; }

    override public string Message => $"Validation errors encountered with {typeof(TCommand).FullName}";
}
