using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using MyGardenPatch.Common;
using MyGardenPatch.Configurations;
using MyGardenPatch.LocalIdentity.Templates;
using MyGardenPatch.Users.Exceptions;
using MyGardenPatch.Users.Services;

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

    public async Task<bool> DoesEmailExist(string emailAddress)
    {
        var user = await _userManager.FindByEmailAsync(emailAddress);

        return user != null;
    }

    public async Task LoginAsync(string emailAddress, string password)
    {
        var result = await _signInManager.PasswordSignInAsync(emailAddress, password, true, false);

        if (!result.Succeeded) throw new LoginAttemptFailedException();
    }

    public async Task RegisterAsync(string fullName, string emailAddress, string password)
    {
        var user = await CreeateUserEntry(fullName, emailAddress);

        await SetPassword(password, user);

        await SendEmailConfirmationRequest(user);
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

    private async Task SetPassword(string password, LocalIdentityUser user)
    {
        var addPasswordResult = await _userManager.AddPasswordAsync(user, password);

        if (!addPasswordResult.Succeeded) throw new InvalidPasswordException();
    }

    private async Task SendEmailConfirmationRequest(LocalIdentityUser user)
    {
        var confirmationToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);

        var emailModel = new
        {
            FullName = user.FullName,
            EmailAddress = user.Email,
            ConfirmationToken = confirmationToken
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
}
