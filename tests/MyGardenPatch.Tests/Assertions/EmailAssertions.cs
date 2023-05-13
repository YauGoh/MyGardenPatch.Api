namespace MyGardenPatch.Tests.Assertions;

public class EmailAssertions
{
    private readonly Mock<IEmailSender> _mockEmailSender;

    public EmailAssertions(Mock<IEmailSender> mockEmailSender)
    {
        _mockEmailSender = mockEmailSender;

        _mockEmailSender
            .Setup(s => s.SendAsync(It.IsAny<Email>()))
            .Returns((Email email) =>
            {
                LastEmailBody = email.HtmlBody;

                return Task.CompletedTask;
            });
    }

    public string? LastEmailBody { get; private set; }
}
