namespace MyGardenPatch.Tests.Users.Queries;

public class GetUserRegistrationStatusQueryTests : TestBase
{
    public GetUserRegistrationStatusQueryTests()
    {
        SeedWith(UserTestData.PeterParker);
    }

    [Fact]
    public async Task RegisteredUser()
    {
        MockCurrentUserProvider
            .Setup(p => p.EmailAddress)
            .Returns(UserTestData.PeterParkerEmailAddress);

        var response = await ExecuteQueryAsync(new GetUserRegistrationStatusQuery());

        response.Status.Should().Be(RegistrationStatus.Registered);
    }

    [Fact]
    public async Task UnregisteredUser()
    {
        MockCurrentUserProvider
            .Setup(p => p.EmailAddress)
            .Returns(UserTestData.UnregisteredEmailAddress);

        var response = await ExecuteQueryAsync(new GetUserRegistrationStatusQuery());

        response.Status.Should().Be(RegistrationStatus.NotRegistered);
    }
}
