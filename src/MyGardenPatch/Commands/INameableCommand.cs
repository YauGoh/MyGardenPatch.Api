using FluentValidation;

namespace MyGardenPatch.Commands;

public interface INameableCommand
{
    public string Name { get; }
}

public static class NameableCommandValidatorExtensions
{
    public static void ValidateNameable<T>(this AbstractValidator<T> validator) where T : INameableCommand
    {
        // Name must be provided and less than 200 characters long
        validator.RuleFor(c => c.Name)
            .NotEmpty().WithMessage("Name is required")
            .MaximumLength(200).WithMessage("Not more than 200 characters");
    }
}
