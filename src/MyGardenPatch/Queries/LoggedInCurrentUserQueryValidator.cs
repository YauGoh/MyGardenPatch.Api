namespace MyGardenPatch.Queries;

public abstract class LoggedInCurrentUserQueryValidator<T> : 
    AbstractValidator<T>, 
    IQueryValidator<T>
{
    public LoggedInCurrentUserQueryValidator(ICurrentUserProvider currentUser)
    {
        RuleFor(_ => _)
            .Must(_ => currentUser.GardenerId is not null)
            .WithMessage("A registered logged in user is required");
    }
}
