namespace MyGardenPatch.Tests.Users.Commands;

public class CreateLocalIdentityUserCommandTests : TestBase
{
    [Fact]
    public async Task CreateLocalIdentityUser()
    {
        var command = new CreateLocalIdentityUserCommand(
            "Tony Stark",
            "tony.stark@email.com",
            "password123",
            "password123");

        await ExecuteCommandAsync(command);

        var identity = await GetIdentityAsync<LocalIdentityUser>(u => u.Email == "tony.stark@email.com");

        identity!.UserName.Should().Be(command.EmailAddress.ToLower());
        identity.Email.Should().Be(command.EmailAddress.ToLower());
        identity.FullName.Should().Be(command.FullName);
        identity.PasswordHash.Should().NotBeNull();
        identity.EmailConfirmed.Should().BeFalse();

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
}