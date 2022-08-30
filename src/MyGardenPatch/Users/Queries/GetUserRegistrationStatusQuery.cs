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

public record UserInfo(UserId? userId, String Name, string EmailAddress);

public record UserRegistrationStatus(RegistrationStatus Status, UserInfo? User) { }

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
        var user = await _users.GetByExpressionAsync(u => u.EmailAddress == _userProvider.EmailAddress, cancellationToken);

        return new UserRegistrationStatus(
            user is null ? RegistrationStatus.NotRegistered : RegistrationStatus.Registered, 
            user is null ? 
                new UserInfo(null, _userProvider.Name!, _userProvider.EmailAddress!)
                : 
                new UserInfo(user.Id, user.Name, user.EmailAddress));
    }
}

public class GetUserRegistrationStatusQueryHandlerValidator : AbstractValidator<GetUserRegistrationStatusQuery>, IQueryValidator<GetUserRegistrationStatusQuery>
{
    public GetUserRegistrationStatusQueryHandlerValidator(ICurrentUserProvider userProvider)
    {
        RuleFor(q => q)
            .Must(q => userProvider.EmailAddress != null)
            .WithMessage("Not logged in with a valid email address");
    }
}
