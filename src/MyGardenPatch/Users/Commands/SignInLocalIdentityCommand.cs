namespace MyGardenPatch.Users.Commands
{
    [Role(WellKnownRoles.Api)]
    public record SignInLocalIdentityCommand(string EmailAddress, string Password) : ICommand;

    public class SignInLocalIdentityCommandHandler : ICommandHandler<SignInLocalIdentityCommand>
    {
        private readonly ILocalIdentityManager _localIdentityManager;

        public SignInLocalIdentityCommandHandler(ILocalIdentityManager localIdentityManager)
        {
            _localIdentityManager = localIdentityManager;
        }

        public async Task HandleAsync(SignInLocalIdentityCommand command, CancellationToken cancellationToken = default)
        {
            await _localIdentityManager.SignInAsync(command.EmailAddress, command.Password);
        }
    }

    public class SignInLocalIdentityCommandValidator : AbstractValidator<SignInLocalIdentityCommand>, ICommandValidator<SignInLocalIdentityCommand>
    {
        public SignInLocalIdentityCommandValidator()
        {
            RuleFor(c => c.EmailAddress)
                .NotEmpty()
                .WithMessage("Email address is required");

            RuleFor(c => c.Password)
                .NotEmpty()
                .WithMessage("Passwor is required");
        }
    }
}
