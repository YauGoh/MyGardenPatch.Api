using MyGardenPatch.Common;
using System.Text.Json.Serialization;

namespace MyGardenPatch.Configurations;

public record EmailConfig
{

    [JsonConstructor]
    public EmailConfig(string smtpServer, int port, EmailAddress systemEmailAddress)
    {
        SmtpServer = smtpServer;
        Port = port;
        SystemEmailAddress = systemEmailAddress;
    }

    public string SmtpServer { get; }
    public int Port { get; }
    public EmailAddress SystemEmailAddress { get; }
}
