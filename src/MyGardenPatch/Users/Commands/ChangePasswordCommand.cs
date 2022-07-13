using MyGardenPatch.Common;

namespace MyGardenPatch.Users.Commands
{
    public record ChangePasswordCommand() : ICommand;

    public class ChangePasswordCommandHandler : ICommandHandler<ChangePasswordCommand>
    {
        private readonly ICurrentUserProvider _currentUser;
        private readonly ILocalIdentityManager _localIdentityManager;

        public ChangePasswordCommandHandler(ICurrentUserProvider currentUser, ILocalIdentityManager localIdentityManager)
        {
            _currentUser = currentUser;
            _localIdentityManager = localIdentityManager;
        }

        public async Task HandleAsync(ChangePasswordCommand command, CancellationToken cancellationToken = default)
        {
            await _localIdentityManager.RequestPasswordResetAsync(_currentUser.CurrentEmailAddress!);
        }
    }

    public class ChangePasswordCommandValidator : AbstractValidator<ChangePasswordCommand>, ICommandValidator<ChangePasswordCommand>
    {
        public ChangePasswordCommandValidator(ICurrentUserProvider currentUser, ILocalIdentityManager localIdentityManager)
        {
            RuleFor(c => c)
                .Cascade(CascadeMode.Stop)
                .Must(c => !string.IsNullOrWhiteSpace(currentUser.CurrentEmailAddress))
                    .WithMessage("Not logged in")
                .MustAsync(async (c, cancellationToken) => await localIdentityManager.DoesEmailExistAsync(currentUser.CurrentEmailAddress))
                    .WithMessage("Not a valid local identity user")
                .MustAsync(async (c, cancellationToken) => await localIdentityManager.IsEmailAddressVerifiedAsync(currentUser.CurrentEmailAddress))
                    .WithMessage("Not a verified local identity");

        }
    }
}
