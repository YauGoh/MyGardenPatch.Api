namespace MyGardenPatch.Users.Exceptions;

public class EmailAddressVerificationException : Exception
{
    public EmailAddressVerificationException(string emailAddress, string errorMessage)
    {
        EmailAddress = emailAddress;
        Message = errorMessage;
    }

    public string EmailAddress { get; }

    public override string Message { get; }
}
