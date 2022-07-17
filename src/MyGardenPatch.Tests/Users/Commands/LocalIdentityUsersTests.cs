using MyGardenPatch.Users.Exceptions;

namespace MyGardenPatch.Tests.Users.Commands;

public class LocalIdentityUsersTests : TestBase
{
    const string FullName = "Tony Stark";
    const string EmailAddress = "tony.start@email.com";
    const string Password = "MyPassword!";
    const string NewPassword = "NEW PASSWORD";

    [Fact]
    public async Task StartNewLocalIdentity()
    {
        var command = new StartNewLocalIdentityCommand(
            FullName,
            EmailAddress);

        await ExecuteCommandAsync(command);

        var identity = await GetIdentityAsync<LocalIdentityUser>(u => u.Email == EmailAddress);

        identity!.UserName.Should().Be(command.EmailAddress.ToLower());
        identity.FullName.Should().Be(command.FullName);

        MockEmailSender.Verify(
            s => s.SendAsync(
                It.Is<Email>(e =>
                    e.To.First().Name == command.FullName &&
                    e.To.First().Address == command.EmailAddress &&
                    e.Subject.Like(new Regex("Please verify your email address")) &&
                    e.HtmlBody.Contains(command.FullName)
                    )),
                Times.Once);
    }

    [Fact]
    public async Task VerifyEmailAddress()
    {
        await StartNewLocalIdentity();

        var regexExtractToken = new Regex("<a[^>]*href='.*verificationToken=([^'&]*)'>");

        var verificationToken = HttpUtility.UrlDecode(regexExtractToken.Match(EmailAssertions.LastEmailBody!).Groups[1].Value);

        verificationToken.Should().NotBeEmpty();

        var command = new VerifyLocalIdentityEmailAddressCommand(
            EmailAddress, 
            verificationToken, 
            Password, 
            Password);

        await ExecuteCommandAsync(command);

        var identity = await GetIdentityAsync<LocalIdentityUser>(u => u.Email == EmailAddress);

        identity!.EmailConfirmed.Should().BeTrue();
        identity.PasswordHash.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task ShouldNotVerifyEmailAddressWithInvalidToken()
    {
        await StartNewLocalIdentity();

        var command = new VerifyLocalIdentityEmailAddressCommand(
            EmailAddress,
            "INVALID TOKEN VALUE",
            Password,
            Password);

        Func<Task> task = () => ExecuteCommandAsync(command);

        await task.Should()
            .ThrowAsync<EmailAddressVerificationException>();
    }

    [Fact]
    public async Task SignIn()
    {
        await VerifyEmailAddress();

        var command = new SignInLocalIdentityCommand(
            EmailAddress,
            Password);

        await ExecuteCommandAsync(command);

        var setCookies = HttpContext.Response.Headers["Set-Cookie"];

        setCookies.Should().Contain(cookie => cookie.Contains("my-garden-patch.auth"));
    }

    [Fact]
    public async Task FailedSignin()
    {
        await VerifyEmailAddress();

        var command = new SignInLocalIdentityCommand(
            EmailAddress,
            "NOT THE RIGHT PASSWORD");

        Func<Task> task = () => ExecuteCommandAsync(command);

        await task.Should().ThrowAsync<LoginAttemptFailedException>();

        var setCookies = HttpContext.Response.Headers["Set-Cookie"];

        setCookies.Should().NotContain(cookie => cookie.Contains("my-garden-patch.auth"));
    }

    [Fact]
    public async Task ShouldLockoutAccountAfterFiveFailedAttempts()
    {
        await VerifyEmailAddress();

        var command = new SignInLocalIdentityCommand(
            EmailAddress,
            "NOT THE RIGHT PASSWORD");

        var maxAllowedAttempts = 5;

        for (var i = 0; i < maxAllowedAttempts; i++)
        {
            Func<Task> task = () => ExecuteCommandAsync(command);
            await task.Should().ThrowAsync<LoginAttemptFailedException>();

            if (i < maxAllowedAttempts - 1)
            {
                var identity = await GetIdentityAsync<LocalIdentityUser>(u => u.Email == EmailAddress);
                identity!.LockoutEnd.Should().BeNull();
            }
        }

        var identityFinal = await GetIdentityAsync<LocalIdentityUser>(u => u.Email == EmailAddress);
        identityFinal!.LockoutEnd.Should().NotBeNull();
    }

    [Fact]
    public async Task RequestForgotPassword()
    {
        await VerifyEmailAddress();

        var command = new RequestForgotPasswordLocalIdentityCommand(
            EmailAddress);

        await ExecuteCommandAsync(command);

        MockEmailSender.Verify(
            s => s.SendAsync(
                It.Is<Email>(e =>
                    e.To.First().Name == FullName &&
                    e.To.First().Address == command.EmailAddress &&
                    e.Subject.Like(new Regex("Password reset request")) &&
                    e.HtmlBody.Contains(FullName)
                    )),
                Times.Once);
    }

    [Fact]
    public async Task ResetPassword()
    {
        await RequestForgotPassword();

        var tokenRegex = new Regex("<a[^>]*href='.*passwordToken=([^'&]*)'>");

        var token = HttpUtility.UrlDecode(tokenRegex.Match(EmailAssertions.LastEmailBody!).Groups[1].Value);

        var command = new ResetPasswordLocalIdentityCommand(
            EmailAddress,
            token,
            NewPassword,
            NewPassword);

        await ExecuteCommandAsync(command);
    }

    [Fact]
    public async Task AfterPasswordResetCanLoginWithNewPassword()
    {
        await ResetPassword();

        var command = new SignInLocalIdentityCommand(
            EmailAddress,
            NewPassword);

        await ExecuteCommandAsync(command);

        var setCookies = HttpContext.Response.Headers["Set-Cookie"];

        setCookies.Should().Contain(cookie => cookie.Contains("my-garden-patch.auth"));
    }

    [Fact]
    public async Task AfterPasswordResetCanNotLoginWithOldPassword()
    {
        await ResetPassword();

        var command = new SignInLocalIdentityCommand(
            EmailAddress,
            Password);

        Func<Task> task = () => ExecuteCommandAsync(command);

        await task.Should().ThrowAsync<LoginAttemptFailedException>();

        var setCookies = HttpContext.Response.Headers["Set-Cookie"];

        setCookies.Should().NotContain(cookie => cookie.Contains("my-garden-patch.auth"));
    }

    [Fact]
    public async Task ChangePassword()
    {
        await VerifyEmailAddress();

        MockCurrentUserProvider.Setup(p => p.CurrentEmailAddress).Returns(EmailAddress);

        var command = new RequestChangePasswordCommand();

        await ExecuteCommandAsync(command);

        MockEmailSender.Verify(
            s => s.SendAsync(
                It.Is<Email>(e =>
                    e.To.First().Name == FullName &&
                    e.To.First().Address == EmailAddress &&
                    e.Subject.Like(new Regex("Password reset request")) &&
                    e.HtmlBody.Contains(FullName)
                    )),
                Times.Once);
    }
}