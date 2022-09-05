namespace MyGardenPatch.Tests.Users.Commands;

public class RegisterUserCommandTests : TestBase
{
    [Fact]
    public async Task RegisterNewUsers()
    {
        SetCurrentUser(UserTestData.UnregisteredUser);

        var date = new DateTime(2022, 1, 1);
        MockDateTimeProvider
            .Setup(p => p.Now)
            .Returns(date);

        await ExecuteCommandAsync(new RegisterGardenerCommand(UserTestData.UnregisteredUser.Name, true, true));

        var user = await GetAsync<Gardener, GardenerId>(u => u.EmailAddress == UserTestData.UnregisteredUser.EmailAddress);

        user!.Name.Should().Be(UserTestData.UnregisteredUser.Name);
        user.EmailAddress.Should().Be(UserTestData.UnregisteredUser.EmailAddress);
        user.RegisteredAt.Should().Be(date);
        user.ReceivesEmail.Should().BeTrue();
    }

    [Fact]
    public async Task RegisterNewUsersRaisesNewUserRegisteredEvent()
    {
        SetCurrentUser(UserTestData.UnregisteredUser);

        var date = new DateTime(2022, 1, 1);
        MockDateTimeProvider
           .Setup(p => p.Now)
           .Returns(date);

        await ExecuteCommandAsync(new RegisterGardenerCommand(UserTestData.UnregisteredUser.Name, true, true));

        MockDomainEventBus.Verify(
            _ => _.PublishAsync(
                It.Is<NewGardenerRegistered>(
                    e => e.RegisteredAt == date),
                default));
    }

    [Theory]
    [InlineData("", true, true, "Name is required", "Name")]
    [InlineData(Strings.Long201, true, true, "Maxmimum length 200 letters", "Name")]
    [InlineData("Peter Parker", true, false, "You must agree to the Terms and Conditions", "AcceptsUserAgreement")]
    public async Task InvalidCommand(
        string name,
        bool receivesEmails,
        bool acceptsUserAgreement,
        string expectedErrorMessage,
        string expectedErrorPropertyPath,
        string because = "")
    {
        SetCurrentUser(UserTestData.UnregisteredUser);
        SeedWith(UserTestData.PeterParker);

        var date = new DateTime(2022, 1, 1);
        MockDateTimeProvider
            .Setup(p => p.Now)
            .Returns(date);

        Func<Task> task = () => ExecuteCommandAsync(new RegisterGardenerCommand(name, receivesEmails, acceptsUserAgreement));

        await task.Should()
            .ThrowAsync<InvalidCommandException<RegisterGardenerCommand>>()
            .WhereHasError(expectedErrorMessage, expectedErrorPropertyPath, because);
    }
}
