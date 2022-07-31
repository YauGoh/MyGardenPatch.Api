namespace MyGardenPatch.Users.Queries;

[Role(WellKnownRoles.Gardener)]
public record GetUserRegistrationStatusQuery : IQuery<UserRegistrationStatus>
{
}

public enum RegistrationStatus
{
    NotRegistered,
    Registered
}

public record UserRegistrationStatus(RegistrationStatus Status) { }

public class GetUserRegistrationStatusQueryHandler : IQueryHandler<GetUserRegistrationStatusQuery, UserRegistrationStatus>
{
    private readonly IRepository<User, UserId> _users;
    private readonly ICurrentUserProvider _userProvider;

    public GetUserRegistrationStatusQueryHandler(IRepository<User, UserId> users, ICurrentUserProvider userProvider)
    {
        _users = users;
        _userProvider = userProvider;
    }

    public async Task<UserRegistrationStatus> HandleAsync(
        GetUserRegistrationStatusQuery query, 
        CancellationToken cancellationToken = default)
    {
        var user = await _users.GetByExpressionAsync(u => u.EmailAddress == _userProvider.CurrentEmailAddress, cancellationToken);

        return new UserRegistrationStatus(user == null ? RegistrationStatus.NotRegistered : RegistrationStatus.Registered);
    }
}

public class GetUserRegistrationStatusQueryHandlerValidator : AbstractValidator<GetUserRegistrationStatusQuery>, IQueryValidator<GetUserRegistrationStatusQuery>
{
    public GetUserRegistrationStatusQueryHandlerValidator(ICurrentUserProvider userProvider)
    {
        RuleFor(q => q)
            .Must(q => userProvider.CurrentEmailAddress != null)
            .WithMessage("Not logged in with a valid email address");
    }
}
