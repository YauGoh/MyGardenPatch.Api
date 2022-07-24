using FluentValidation;

namespace MyGardenPatch.Commands;

public interface IImageableCommand : ICommand
{
    Uri? ImageUri { get; }
    string? ImageDescription { get; }
}

public static class ImageableCommandValidatorExtensions
{
    public static void ValidateImageable<T>(this AbstractValidator<T> validator) where T : IImageableCommand
    {
        validator.When(c => c.ImageUri is not null,
            () =>
            {
                // Name must be provided and less than 200 characters long
                validator.RuleFor(c => c.ImageUri)
                    .Must(uri => uri.AbsoluteUri.Length <= 200)
                    .WithMessage("Not more than 200 characters");
            });
    }
}
