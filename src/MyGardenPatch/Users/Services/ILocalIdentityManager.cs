namespace MyGardenPatch.Users.Services
{
    public interface ILocalIdentityManager
    {
        Task<bool> DoesEmailExist(string emailAddress);

        Task RegisterAsync(string fullName, string emailAddress, string password);

        Task LoginAsync(string emailAddress, string password);
    }
}
