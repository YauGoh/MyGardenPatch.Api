using Microsoft.Extensions.Options;
using MyGardenPatch.Configurations;
using System.Net.Mail;

namespace MyGardenPatch.Common
{
    public interface IEmailSender
    {
        Task SendAsync(Email email);
    }

    public record EmailAddress(string Address, string Name);

    public record Email(IEnumerable<EmailAddress> To, IEnumerable<EmailAddress> Cc, IEnumerable<EmailAddress> Bcc, EmailAddress From, string Subject, string HtmlBody, string? TextBody)
    {
        public Email(EmailAddress to, EmailAddress from, string subject, string htmlBody) : this(
            new[] { to },
            Enumerable.Empty<EmailAddress>(),
            Enumerable.Empty<EmailAddress>(),
            from,
            subject,
            htmlBody,
            null)
        { }
    }

    internal class SmtpEmailSender : IEmailSender
    {
        private readonly IOptions<EmailConfig> _emailConfig;

        public SmtpEmailSender(IOptions<EmailConfig> emailConfig)
        {
            _emailConfig = emailConfig;
        }

        public Task SendAsync(Email email)
        {
            return Task.Run(() =>
            {
                using var smtpClient = new SmtpClient(_emailConfig.Value.SmtpServer, _emailConfig.Value.Port);

                smtpClient.Send(GetMailMessage(email));
            })
            .ContinueWith(t => { if (t.Exception != null) throw t.Exception; });
        }

        private MailMessage GetMailMessage(Email email)
        {
            var message = new MailMessage();

            foreach (var to in email.To)
            {
                message.To.Add(new MailAddress(to.Address, to.Name));
            }

            foreach (var cc in email.Cc)
            {
                message.CC.Add(new MailAddress(cc.Address, cc.Name));
            }

            foreach (var bcc in email.Bcc)
            {
                message.Bcc.Add(new MailAddress(bcc.Address, bcc.Name));
            }

            message.Subject = email.Subject;

            message.Body = email.HtmlBody;
            message.IsBodyHtml = true;

            return message;
        }
    }
}
