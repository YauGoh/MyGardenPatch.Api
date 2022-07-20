namespace MyGardenPatch.Users.Commands;

[Role(WellKnownRoles.Api)]
public record VerifyLocalIdentityEmailAddressCommand(
    string EmailAddress, 
    string VerificationToken,
    string Password,
    string PasswordConfirm) : ICommand;

public class VerifyLocalIdentityEmailAddressCommandHandler : ICommandHandler<VerifyLocalIdentityEmailAddressCommand>
{
    private readonly ILocalIdentityManager _localIdentityManager;

    public VerifyLocalIdentityEmailAddressCommandHandler(
        ILocalIdentityManager localIdentityManager)
    {
        _localIdentityManager = localIdentityManager;
    }

    public async Task HandleAsync(
        VerifyLocalIdentityEmailAddressCommand command, 
        CancellationToken cancellationToken = default)
    {
        await _localIdentityManager.VerifyEmailAddressAsync(
            command.EmailAddress,
            command.Password,
            command.VerificationToken);
    }
}

public class VerifyLocalIdentiyEmailAddressCommandValidator : 
    AbstractValidator<VerifyLocalIdentityEmailAddressCommand>, 
    ICommandValidator<VerifyLocalIdentityEmailAddressCommand>
{
    public VerifyLocalIdentiyEmailAddressCommandValidator(ILocalIdentityManager localIdentityManager)
    {
        RuleFor(c => c.EmailAddress)
            .Cascade(CascadeMode.Stop)
            .EmailAddress()
                .WithMessage("A valid email address is required")
            .NotEmpty()
                .WithMessage("An email address is required")
            .MustAsync(async (emailAddress, cancellationToken) => await localIdentityManager.DoesEmailExistAsync(emailAddress))
                .WithMessage("Email address does not exist")
            .MustAsync(async (emailAddress, cancellationToken) => !(await localIdentityManager.IsEmailAddressVerifiedAsync(emailAddress)))
                .WithMessage("Email address is already verified");

        RuleFor(c => c.VerificationToken)
            .NotEmpty()
                .WithMessage("A verification token is required");

        RuleFor(c => c.Password)
            .NotEmpty()
                .WithMessage("A password is required");
           

        RuleFor(c => c.PasswordConfirm)
             .Must((c, passwordConfirm) => c.Password == passwordConfirm)
                .WithMessage("The passwords does not match");
    }
}
