namespace MyGardenPatch.Users.Commands;

[Role(WellKnownRoles.Gardener)]
public record RegisterUserCommand(
    string Name, 
    bool ReceivesEmails,
    bool AcceptsUserAgreement) : ICommand;

public class RegisterUserCommandHandler : ICommandHandler<RegisterUserCommand>
{
    private readonly ICurrentUserProvider _userProvider;
    private readonly IDateTimeProvider _dateTime;
    private readonly IRepository<User, UserId> _users;

    public RegisterUserCommandHandler(ICurrentUserProvider userProvider, IDateTimeProvider dateTime, IRepository<User, UserId> users)
    {
        _userProvider = userProvider;
        _dateTime = dateTime;
        _users = users;
    }

    public async Task HandleAsync(
        RegisterUserCommand command, 
        CancellationToken cancellationToken = default)
    {
        var user = new User(
                command.Name,
                _userProvider.EmailAddress!.ToLower());

        user!.Register(
            _dateTime.Now, 
            command.ReceivesEmails);

        await _users.AddOrUpdateAsync(user);
    }
}

public class RegisterUserCommandValidator : AbstractValidator<RegisterUserCommand>, ICommandValidator<RegisterUserCommand>
{
    public RegisterUserCommandValidator(ICurrentUserProvider userProvider, IRepository<User, UserId> users)
    {

        RuleFor(command => command.AcceptsUserAgreement)
            .Must(aceepts => aceepts == true)
            .WithMessage("You must agree to the Terms and Conditions");

        RuleFor(command => command)
            .Must(command => !string.IsNullOrEmpty(userProvider.EmailAddress))
                .WithMessage("Must be logged in with a valid email address")
            .MustAsync(async(command, cancellationToken) => !(await users.AnyAsync(u => u.EmailAddress.ToLower() == userProvider.EmailAddress!.ToLower())))
                .WithMessage("User with email address is already registered");

        RuleFor(command => command.Name)
            .NotEmpty().WithMessage("Name is required")
            .MaximumLength(200).WithMessage("Maxmimum length 200 letters");
    }
}
