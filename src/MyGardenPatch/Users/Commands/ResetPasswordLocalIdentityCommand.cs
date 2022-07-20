namespace MyGardenPatch.Users.Commands;

[Role(WellKnownRoles.Api)]
public record ResetPasswordLocalIdentityCommand(
    string EmailAddress, 
    string PasswordResetToken, 
    string Password, 
    string PasswordConfirm) : ICommand;

public class ResetPasswordLocalIdentityCommandHandler : ICommandHandler<ResetPasswordLocalIdentityCommand>
{
    private readonly ILocalIdentityManager _localIdentityManager;

    public ResetPasswordLocalIdentityCommandHandler(ILocalIdentityManager localIdentityManager)
    {
        _localIdentityManager = localIdentityManager;
    }

    public async Task HandleAsync(ResetPasswordLocalIdentityCommand command, CancellationToken cancellationToken = default)
    {
        await _localIdentityManager.ResetPasswordAsync(
            command.EmailAddress, 
            command.PasswordResetToken, 
            command.Password);
    }
}

public class ResetPasswordLocalIdentityCommandValidator : AbstractValidator<ResetPasswordLocalIdentityCommand>, ICommandValidator<ResetPasswordLocalIdentityCommand>
{
    public ResetPasswordLocalIdentityCommandValidator()
    {
        RuleFor(c => c.EmailAddress)
            .NotEmpty()
                .WithMessage("An email address is required")
            .EmailAddress()
                .WithMessage("A valid email address is required");

        RuleFor(c => c.PasswordResetToken)
            .NotEmpty()
                .WithMessage("A reset token is required");

        RuleFor(c => c.Password)
            .NotEmpty()
                .WithMessage("A password is required");

        RuleFor(c => c.PasswordConfirm)
            .Must((command, passwordConfirm) => command.Password == passwordConfirm)
                .WithMessage("The passwords must be the same");
    }
}
