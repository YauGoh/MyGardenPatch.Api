namespace MyGardenPatch.Users.Commands;

[Role(WellKnownRoles.Api)]
public record StartNewLocalIdentityCommand(string Name, string EmailAddress) : ICommand;

public class StartNewLocalIdentityCommandHandler : ICommandHandler<StartNewLocalIdentityCommand>
{
    private readonly ILocalIdentityManager _localIdentityManager;

    public StartNewLocalIdentityCommandHandler(ILocalIdentityManager localIdentityManager)
    {
        _localIdentityManager = localIdentityManager;
    }

    public async Task HandleAsync(
        StartNewLocalIdentityCommand command, 
        CancellationToken cancellationToken = default)
    {
        await _localIdentityManager.StartLocalIdentityRegistrationAsync(
            command.Name, 
            command.EmailAddress, 
            cancellationToken);
    }
}

public class StartNewLocalIdentityCommandValidator : AbstractValidator<StartNewLocalIdentityCommand>,  ICommandValidator<StartNewLocalIdentityCommand>
{
    public StartNewLocalIdentityCommandValidator(
        ILocalIdentityManager localIdentityManager, 
        IRepository<User, UserId> users)
    {
        RuleFor(c => c.Name)
            .NotEmpty()
                .WithMessage("A name is required");

        RuleFor(c => c.EmailAddress)
            .MustAsync(async (emailAddress, cancellationToken) => !(await localIdentityManager.DoesEmailExistAsync(emailAddress)))
                .WithMessage("Email is already registered")
            .MustAsync(async (emailAddress, cancellationToken) => !(await users.AnyAsync(u => u.EmailAddress == emailAddress, cancellationToken)))
                .WithMessage("Email is already registered");
    }
}

