namespace MyGardenPatch.Users.Commands;

[Role(WellKnownRoles.Api)]
public record RequestForgotPasswordLocalIdentityCommand(string EmailAddress) : ICommand;

public class RequestForgotPasswordLocalIdentityCommandHandler : ICommandHandler<RequestForgotPasswordLocalIdentityCommand>
{
    private readonly ILocalIdentityManager _localIdentityManager;

    public RequestForgotPasswordLocalIdentityCommandHandler(ILocalIdentityManager localIdentityManager)
    {
        _localIdentityManager = localIdentityManager;
    }

    public Task HandleAsync(
        RequestForgotPasswordLocalIdentityCommand command, 
        CancellationToken cancellationToken = default)
    {
        return _localIdentityManager.RequestPasswordResetAsync(command.EmailAddress);
    }
}

public class RequestForgotPasswordLocalIdentityCommandValidator : 
    AbstractValidator<RequestForgotPasswordLocalIdentityCommand>, 
    ICommandValidator<RequestForgotPasswordLocalIdentityCommand>
{
    public RequestForgotPasswordLocalIdentityCommandValidator(ILocalIdentityManager localIdentityManager)
    {
        RuleFor(c => c.EmailAddress)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
                .WithMessage("An email address is required")
            .EmailAddress()
                .WithMessage("A valid email address is required");
            //.MustAsync((emailAddress, cancellationToken) => localIdentityManager.DoesEmailExistAsync(emailAddress))
            //    .WithMessage("The email address does not exist")
            //.MustAsync((emailAddress, cancellationToken) => localIdentityManager.IsEmailAddressVerifiedAsync(emailAddress))
            //    .WithMessage("The email address has not been verified");
    }
}
