namespace MyGardenPatch.Users.Exceptions;

public class InvalidPasswordException : Exception
{
    public InvalidPasswordException(string localIdentityUserId, string message)
    {
        LocalIdentityUserId = localIdentityUserId;
        Message = message;
    }

    public string LocalIdentityUserId { get; }

    public override string Message { get; }
}
