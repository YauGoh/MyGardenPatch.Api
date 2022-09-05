namespace MyGardenPatch.Gardeners.Commands;

[Role(WellKnownRoles.Gardener)]
public record RegisterGardenerCommand(
    string Name,
    bool ReceivesEmails,
    bool AcceptsUserAgreement) : ICommand;

public class RegisterGardnerCommandHandler : ICommandHandler<RegisterGardenerCommand>
{
    private readonly ICurrentUserProvider _userProvider;
    private readonly IDateTimeProvider _dateTime;
    private readonly IRepository<Gardener, GardenerId> _gardener;

    public RegisterGardnerCommandHandler(
        ICurrentUserProvider userProvider, 
        IDateTimeProvider dateTime, 
        IRepository<Gardener, GardenerId> gardener)
    {
        _userProvider = userProvider;
        _dateTime = dateTime;
        _gardener = gardener;
    }

    public async Task HandleAsync(
        RegisterGardenerCommand command,
        CancellationToken cancellationToken = default)
    {
        var user = new Gardener(
                command.Name,
                _userProvider.EmailAddress!.ToLower());

        user!.Register(
            _dateTime.Now,
            command.ReceivesEmails);

        await _gardener.AddOrUpdateAsync(user);
    }
}

public class RegisterGardenerCommandValidator : AbstractValidator<RegisterGardenerCommand>, ICommandValidator<RegisterGardenerCommand>
{
    public RegisterGardenerCommandValidator(
        ICurrentUserProvider userProvider, 
        IRepository<Gardener, GardenerId> gardener)
    {

        RuleFor(command => command.AcceptsUserAgreement)
            .Must(aceepts => aceepts == true)
            .WithMessage("You must agree to the Terms and Conditions");

        RuleFor(command => command)
            .Must(command => !string.IsNullOrEmpty(userProvider.EmailAddress))
                .WithMessage("Must be logged in with a valid email address")
            .MustAsync(async (command, cancellationToken) => !await gardener.AnyAsync(u => u.EmailAddress.ToLower() == userProvider.EmailAddress!.ToLower()))
                .WithMessage("User with email address is already registered");

        RuleFor(command => command.Name)
            .NotEmpty().WithMessage("Name is required")
            .MaximumLength(200).WithMessage("Maxmimum length 200 letters");
    }
}
