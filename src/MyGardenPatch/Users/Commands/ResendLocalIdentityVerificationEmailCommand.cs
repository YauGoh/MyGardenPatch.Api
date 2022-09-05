namespace MyGardenPatch.Users.Commands
{
    [Role(WellKnownRoles.Api)]
    public record ResendLocalIdentityVerificationEmailCommand(string EmailAddress) : ICommand;

    public class ResendLocalIdentityVerificationEmailCommandHandler : ICommandHandler<ResendLocalIdentityVerificationEmailCommand>
    {
        private readonly ILocalIdentityManager _localIdentityManager;

        public ResendLocalIdentityVerificationEmailCommandHandler(ILocalIdentityManager localIdentityManager)
        {
            _localIdentityManager = localIdentityManager;
        }

        public async Task HandleAsync(ResendLocalIdentityVerificationEmailCommand command, CancellationToken cancellationToken = default)
        {
            await _localIdentityManager.RequestRegistrationResetAsync(command.EmailAddress, cancellationToken);
        }
    }

    public class ResendLocalIdentityVerificationEmailValidator : 
        AbstractValidator<ResendLocalIdentityVerificationEmailCommand>, 
        ICommandValidator<ResendLocalIdentityVerificationEmailCommand>
    {
        public ResendLocalIdentityVerificationEmailValidator(ILocalIdentityManager localIdentityManager)
        {
            RuleFor(command => command.EmailAddress)
                .MustAsync(async (emailAddress, cancellationToken) => !(await localIdentityManager.IsEmailAddressVerifiedAsync(emailAddress)));
        }
    }
}
