namespace MyGardenPatch.Users.Services;

public interface ILocalIdentityManager
{
    Task<bool> DoesEmailExistAsync(string emailAddress);

    Task StartLocalIdentityRegistrationAsync(string fullName, string emailAddress, CancellationToken cancellationToken);

    Task<bool> IsEmailAddressVerifiedAsync(string emailAddress);

    Task VerifyEmailAddressAsync(string emailAddress, string password, string verificationToken);

    Task SignInAsync(string emailAddress, string password);

    Task RequestPasswordResetAsync(string emailAddress);

    Task ResetPasswordAsync(string emailAddress, string passwordResetToken, string password);
}
