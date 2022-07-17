namespace MyGardenPatch.Configurations;

public record EmailConfig
{
    public EmailConfig() { }
    
    public EmailConfig(string smtpServer, int port, EmailAddress systemEmailAddress)
    {
        SmtpServer = smtpServer;
        Port = port;
        SystemEmailAddress = systemEmailAddress;
    }

    public string SmtpServer { get; init; } = String.Empty;
    public int Port { get; init; }
    public EmailAddress SystemEmailAddress { get; init; } = new();
}
