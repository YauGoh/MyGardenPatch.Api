namespace MyGardenPatch.Users.Queries;

[Role(WellKnownRoles.Gardener)]
public record GetLoggedInIdentityQuery : IQuery<LoggedInIdentity>;

public record LoggedInIdentity(GardenerId? GardenerId, string Name, string EmailAddress);

public class GetLoggedInIdentityQueryHandler : IQueryHandler<GetLoggedInIdentityQuery, LoggedInIdentity>
{
    private readonly ICurrentUserProvider _currentUser;

    public GetLoggedInIdentityQueryHandler(
        ICurrentUserProvider currentUser)
    {
        _currentUser = currentUser;
    }

    public Task<LoggedInIdentity> HandleAsync(GetLoggedInIdentityQuery query, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(
            new LoggedInIdentity(
                _currentUser.GardenerId, 
                _currentUser.Name!, 
                _currentUser.EmailAddress!));
    }
}

public class GetLoggedInIdentityQueryValidator :
    AbstractValidator<GetLoggedInIdentityQuery>, 
    IQueryValidator<GetLoggedInIdentityQuery>
{
    public GetLoggedInIdentityQueryValidator(ICurrentUserProvider currentUser)
    {
        RuleFor(_ => _)
            .Must(_ => currentUser.EmailAddress is not null)
            .WithMessage("No user is currently logged in.");
    }
}
