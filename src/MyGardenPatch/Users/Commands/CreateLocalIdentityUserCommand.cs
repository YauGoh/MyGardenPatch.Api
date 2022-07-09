using FluentValidation;
using MyGardenPatch.Aggregates;
using MyGardenPatch.Commands;
using MyGardenPatch.Common;
using MyGardenPatch.Users.Services;

namespace MyGardenPatch.Users.Commands
{
    public record CreateLocalIdentityUserCommand(string FullName, string EmailAddress, string Password, string PasswordConfirm) : ICommand;

    public class CreateLocalIdentityUserCommandHandler : ICommandHandler<CreateLocalIdentityUserCommand>
    {
        private readonly ILocalIdentityManager _localIdentityManager;

        public CreateLocalIdentityUserCommandHandler(
            ILocalIdentityManager localIdentityManager,
            IRepository<User, UserId> users,
            IDateTimeProvider dateTimeProvider)
        {
            _localIdentityManager = localIdentityManager;
        }

        public async Task HandleAsync(CreateLocalIdentityUserCommand command, CancellationToken cancellationToken = default)
        {
            await _localIdentityManager.RegisterAsync(command.FullName, command.EmailAddress, command.Password);
        }
    }

    public class CreateLocalIdentityUserCommandValidator : AbstractValidator<CreateLocalIdentityUserCommand>, ICommandValidator<CreateLocalIdentityUserCommand>
    {
        public CreateLocalIdentityUserCommandValidator(IRepository<User, UserId> users)
        {
            RuleFor(c => c.FullName)
                .NotEmpty()
                    .WithMessage("A name is required")
                .MaximumLength(200)
                    .WithMessage("Not more than 200 characters");

            RuleFor(c => c.EmailAddress)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                    .WithMessage("Email address is required")
                .EmailAddress()
                    .WithMessage("Must be a valid email address")
                .MustAsync(async (e, c) => !(await users.AnyAsync(u => u.EmailAddress == e)))
                    .WithMessage("An account with this email address is already registered");

            RuleFor(c => c.PasswordConfirm)
                .Must((c, p) => c.Password == p)
                .WithMessage("Passwords must match");
        }
    }
}
