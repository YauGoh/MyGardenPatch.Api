namespace MyGardenPatch.WebApi.Tests.TestFixtureExtensions;

record CreateUser(string FullName, string EmailAddress, string Password);

internal static class UsersExtensions
{
    internal static TestFixture WithUser(
        this TestFixture testFixture, 
        string fullName, 
        string emailAddress, 
        string password)
    {
        var users = testFixture.GetState<List<CreateUser>>() ?? new List<CreateUser>();

        users.Add(new CreateUser(fullName, emailAddress, password));

        testFixture.SetState(users);

        return testFixture;
    }

    internal static TestFixture ClearUsers(
        this TestFixture testFixture)
    {
        var users = testFixture.GetState<List<CreateUser>>() ?? new List<CreateUser>();

        users.Clear();

        testFixture.SetState(users);

        return testFixture;
    }

    internal static List<CreateUser> GetCreateUsers(this TestFixture testFixture)
        => testFixture.GetState<List<CreateUser>>() ?? new List<CreateUser>();
}
