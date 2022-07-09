using FluentValidation.Results;

namespace MyGardenPatch.Commands
{
    internal class InvalidCommandException<TCommand> : Exception
    {
        public InvalidCommandException(TCommand command, List<ValidationFailure> validationErrors)
        {
            Command = command;
            ValidationErrors = validationErrors;
        }

        public TCommand Command { get; }

        public List<ValidationFailure> ValidationErrors { get; }

        override public string Message => $"Validation errors encountered with {typeof(TCommand).FullName}";
    }
}
