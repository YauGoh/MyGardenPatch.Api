namespace MyGardenPatch.Users.Exceptions;

public class InvalidPasswordException : Exception
{
    public InvalidPasswordException(Guid localIdentityUserId, string message)
    {
        LocalIdentityUserId = localIdentityUserId;
        Message = message;
    }

    public Guid LocalIdentityUserId { get; }

    public override string Message { get; }
}
