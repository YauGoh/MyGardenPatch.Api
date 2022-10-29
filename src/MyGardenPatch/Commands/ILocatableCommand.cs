namespace MyGardenPatch.Commands;

public interface ILocateableCommand
{
    Point Center { get; }
}

public static class LocateableCommandValidatorExtensions
{
    public static void ValidateLocatable<T>(this AbstractValidator<T> validator) where T : ILocateableCommand
    {
        validator.RuleFor(c => c.Center)
           .NotEmpty()
           .WithMessage("A location is required");
    }
}
