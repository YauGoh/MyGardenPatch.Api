using MyGardenPatch.Common;

namespace MyGardenPatch.Users.Commands
{
    public record RequestChangePasswordLocalIdentityCommand() : ICommand;

    public class RequestChangePasswordLocalIdentityCommandHandler : ICommandHandler<RequestChangePasswordLocalIdentityCommand>
    {
        private readonly ICurrentUserProvider _currentUser;
        private readonly ILocalIdentityManager _localIdentityManager;

        public RequestChangePasswordLocalIdentityCommandHandler(
            ICurrentUserProvider currentUser, 
            ILocalIdentityManager localIdentityManager)
        {
            _currentUser = currentUser;
            _localIdentityManager = localIdentityManager;
        }

        public async Task HandleAsync(
            RequestChangePasswordLocalIdentityCommand command, 
            CancellationToken cancellationToken = default)
        {
            await _localIdentityManager.RequestPasswordResetAsync(_currentUser.CurrentEmailAddress!);
        }
    }

    public class RequestChangePasswordLocalIdentityCommandValidator : 
        AbstractValidator<RequestChangePasswordLocalIdentityCommand>, 
        ICommandValidator<RequestChangePasswordLocalIdentityCommand>
    {
        public RequestChangePasswordLocalIdentityCommandValidator(ICurrentUserProvider currentUser, ILocalIdentityManager localIdentityManager)
        {
            RuleFor(c => c)
                .Cascade(CascadeMode.Stop)
                .Must(c => !string.IsNullOrWhiteSpace(currentUser.CurrentEmailAddress))
                    .WithMessage("Not logged in")
                .MustAsync(async (c, cancellationToken) => await localIdentityManager.DoesEmailExistAsync(currentUser.CurrentEmailAddress!))
                    .WithMessage("Not a valid local identity user")
                .MustAsync(async (c, cancellationToken) => await localIdentityManager.IsEmailAddressVerifiedAsync(currentUser.CurrentEmailAddress!))
                    .WithMessage("Not a verified local identity");

        }
    }
}
