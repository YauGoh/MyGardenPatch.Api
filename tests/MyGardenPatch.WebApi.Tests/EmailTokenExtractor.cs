using System.Web;

namespace MyGardenPatch.WebApi.Tests;
internal class EmailTokenExtractor
{
    internal EmailTokenExtractor(Mock<IEmailSender> mockSender, Regex tokenExpression)
    {

        mockSender
            .Setup(s => s.SendAsync(It.IsAny<Email>()))
            .Returns((Email email) =>
            {
                if (tokenExpression.IsMatch(email.HtmlBody))
                {
                    Token = HttpUtility.UrlDecode(tokenExpression.Match(email.HtmlBody).Groups[1].Value);
                }

                return Task.CompletedTask;
            });
    }

    internal string Token { get; private set; } = string.Empty;
}
