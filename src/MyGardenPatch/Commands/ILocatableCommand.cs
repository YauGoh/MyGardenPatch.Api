using FluentValidation;
using MyGardenPatch.Common;

namespace MyGardenPatch.Commands
{
    public interface ILocateableCommand
    {
        Location Location { get; }
    }

    public static class LocateableCommandValidatorExtensions
    {
        public static void ValidateLocatable<T>(this AbstractValidator<T> validator) where T : ILocateableCommand
        {
            validator.RuleFor(c => c.Location)
               .NotEmpty().WithMessage("A position/boundary is required")
               .Must(l => (l.Type == LocationType.Point || l.Type == LocationType.Boundary) && l.Points.Any()).WithMessage("A valid point or boundary is required");
        }
    }
}
