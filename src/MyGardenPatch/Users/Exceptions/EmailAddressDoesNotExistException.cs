namespace MyGardenPatch.Users.Exceptions;

public class EmailAddressDoesNotExistException : Exception
{
    public EmailAddressDoesNotExistException(string emailAddress)
    {
        EmailAddress = emailAddress;
    }

    public string EmailAddress { get; }

    public override string Message => $"{EmailAddress} does not exist.";
}
