namespace MyGardenPatch.Users.Commands;

public interface IUserCommand : ICommand
{
    UserId UserId { get; }
}
