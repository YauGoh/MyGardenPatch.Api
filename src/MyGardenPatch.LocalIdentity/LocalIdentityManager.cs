namespace MyGardenPatch.LocalIdentity;

internal class LocalIdentityMananger : ILocalIdentityManager
{
    private readonly UserManager<LocalIdentityUser> _userManager;
    private readonly SignInManager<LocalIdentityUser> _signInManager;
    private readonly IEmailSender _emailSender;
    private readonly IOptions<EmailConfig> _emailConfig;

    public LocalIdentityMananger(
        UserManager<LocalIdentityUser> userManager,
        SignInManager<LocalIdentityUser> signInManager,
        IEmailSender emailSender,
        IOptions<EmailConfig> emailConfig)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _emailSender = emailSender;
        _emailConfig = emailConfig;
    }

    public async Task<bool> DoesEmailExistAsync(string emailAddress)
    {
        var user = await _userManager.FindByEmailAsync(emailAddress);

        return user != null;
    }

    public async Task StartLocalIdentityRegistrationAsync(string fullName, string emailAddress, CancellationToken cancellationToken)
    {
        var user = await CreeateUserEntry(fullName, emailAddress);

        await SendEmailConfirmationRequestAsync(user);
    }

    public async Task<bool> IsEmailAddressVerifiedAsync(string emailAddress)
    {
        var user = await _userManager.FindByEmailAsync(emailAddress);

        if (user == null) return false;

        return user.EmailConfirmed;
    }

    public async Task VerifyEmailAddressAsync(string emailAddress, string password, string verificationToken)
    {
        var isVerified = await IsEmailAddressVerifiedAsync(emailAddress);

        if (isVerified) throw new EmailAddressHasAlreadyVerifiedException(emailAddress);

        var user = await _userManager.FindByEmailAsync(emailAddress);

        if (user == null) throw new EmailAddressDoesNotExistException(emailAddress);

        var result = await _userManager.ConfirmEmailAsync(user, verificationToken);

        if (!result.Succeeded) throw new EmailAddressVerificationException(
            emailAddress, 
            string.Join("; ", result.Errors.Select(e => e.Description)));

        var passwordResult = await _userManager.AddPasswordAsync(user, password);

        if (!passwordResult.Succeeded) throw new InvalidPasswordException(user.Id, string.Join("; ", passwordResult.Errors.Select(e => e.Description)));

        await _userManager.AddToRoleAsync(user, WellKnownRoles.Gardener);
    }

    public async Task SignInAsync(string emailAddress, string password)
    {
        var result = await _signInManager.PasswordSignInAsync(emailAddress, password, true, true);

        if (!result.Succeeded) throw new LoginAttemptFailedException();
    }

    public async Task RequestPasswordResetAsync(string emailAddress)
    {
        var user = await _userManager.FindByEmailAsync(emailAddress);

        if (user == null) return;

        if (!user.EmailConfirmed) return;
        await SendPasswordResetEmailAsync(user);
    }

    public async Task ResetPasswordAsync(string emailAddress, string passwordResetToken, string password)
    {
        var user = await _userManager.FindByEmailAsync(emailAddress);

        if (user == null) return;

        await _userManager.ResetPasswordAsync(user, passwordResetToken, password);
    }

    private async Task<LocalIdentityUser> CreeateUserEntry(string fullName, string emailAddress)
    {
        var user = new LocalIdentityUser
        {
            UserName = emailAddress.ToLower(),
            FullName = fullName,
            Email = emailAddress.ToLower()
        };

        var result = await _userManager.CreateAsync(user);

        if (!result.Succeeded) throw new UserRegistrationException();
        return user;
    }

    private async Task SendEmailConfirmationRequestAsync(LocalIdentityUser user)
    {
        var verificationToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);

        var emailModel = new
        {
            FullName = user.FullName,
            EmailAddress = user.Email,
            VerificationUrl= $"https://baseurl?emailAddress={HttpUtility.UrlEncode(user.Email)}&verificationToken={HttpUtility.UrlEncode(verificationToken)}"
        };

        var subject = await ResourceTemplates.Render("EmailConfirmation.subject.liquid", emailModel);
        var htmlBody = await ResourceTemplates.Render("EmailConfirmation.body.liquid", emailModel);

        await _emailSender.SendAsync(
            new Email(
                new EmailAddress(
                    user.Email,
                    user.FullName),
                _emailConfig.Value.SystemEmailAddress,
                subject,
                htmlBody));
    }

    private async Task SendPasswordResetEmailAsync(LocalIdentityUser user)
    {
        var passwordToken = await _userManager.GeneratePasswordResetTokenAsync(user);

        var emailModel = new
        {
            FullName = user.FullName,
            EmailAddress = user.Email,
            PasswordResetUrl = $"https://baseurl?emailAddress={HttpUtility.UrlEncode(user.Email)}&passwordToken={HttpUtility.UrlEncode(passwordToken)}"
        };

        var subject = await ResourceTemplates.Render("PasswordReset.subject.liquid", emailModel);
        var htmlBody = await ResourceTemplates.Render("PasswordReset.body.liquid", emailModel);

        await _emailSender.SendAsync(
            new Email(
                new EmailAddress(
                    user.Email,
                    user.FullName),
                _emailConfig.Value.SystemEmailAddress,
                subject,
                htmlBody));
    }

    
}
