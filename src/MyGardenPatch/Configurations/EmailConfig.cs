using MyGardenPatch.Common;

namespace MyGardenPatch.Configurations;

public record EmailConfig(string SmtpServer, int Port, EmailAddress SystemEmailAddress);
