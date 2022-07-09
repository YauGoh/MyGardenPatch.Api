using FluentValidation;
using MyGardenPatch.Aggregates;
using MyGardenPatch.Commands;

namespace MyGardenPatch.Users.Commands
{
    public record RegisterUserCommand(string Name, string EmailAddress, DateTime RegisteredAt, bool ReceivesEmails) : ICommand;

    public class RegisterUserCommandHandler : ICommandHandler<RegisterUserCommand>
    {
        private readonly IRepository<User, UserId> _users;

        public RegisterUserCommandHandler(IRepository<User, UserId> users)
        {
            _users = users;
        }

        public async Task HandleAsync(RegisterUserCommand command, CancellationToken cancellationToken = default)
        {
            var user = await _users.GetByExpressionAsync(u => u.EmailAddress == command.EmailAddress);

            if (user == null)
            {
                user = new User(command.Name, command.EmailAddress);
            }

            user!.Register(command.RegisteredAt, command.ReceivesEmails);

            await _users.AddOrUpdateAsync(user);
        }
    }

    public class RegisterUserCommandValidator : AbstractValidator<RegisterUserCommand>, ICommandValidator<RegisterUserCommand>
    {
        public RegisterUserCommandValidator(IRepository<User, UserId> users)
        {
            RuleFor(command => command.Name)
                .NotEmpty().WithMessage("Name is required")
                .MaximumLength(200).WithMessage("Maxmimum length 200 letters");

            RuleFor(command => command.EmailAddress)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("Email address is required")
                .EmailAddress().WithMessage("Invalid Email address")
                .MaximumLength(200).WithMessage("Maxmimum length 200 letters")
                .MustAsync(async (emailAddress, cancelationToken) => (await users.GetByExpressionAsync(u => u.EmailAddress == emailAddress)) == null)
                .WithMessage("User with email address is already registered");
        }
    }
}
