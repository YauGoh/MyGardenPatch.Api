namespace MyGardenPatch.Users.Exceptions;

public class EmailAddressHasAlreadyVerifiedException : Exception
{
    public EmailAddressHasAlreadyVerifiedException(string emailAddress)
    {
        EmailAddress = emailAddress;
    }

    public string EmailAddress { get; }

    public override string Message => $"{EmailAddress} has already been verified";
}
