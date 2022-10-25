namespace MyGardenPatch.Commands;

public interface IShapeableCommand
{
    Shape Shape { get; }
}

public static class ShapableCommandValidatorExtensions
{
    public static void ValidateShape<T>(this AbstractValidator<T> validator) where T : IShapeableCommand
    {
        // Name must be provided and less than 200 characters long
        validator.RuleFor(c => c.Shape)
            .NotNull().WithMessage("A shape is required");
    }
}