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

        await ExecuteCommandAsync(new RegisterUserCommand(UserTestData.UnregisteredUser.Name, true));

        var user = await GetAsync<User, UserId>(u => u.EmailAddress == UserTestData.UnregisteredUser.EmailAddress);

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

        await ExecuteCommandAsync(new RegisterUserCommand(UserTestData.UnregisteredUser.Name, true));

        MockDomainEventBus.Verify(
            _ => _.PublishAsync(
                It.Is<NewUserRegistered>(
                    e => e.RegisteredAt == date),
                default));
    }

    [Theory]
    [InlineData("", true, "Name is required", "Name")]
    [InlineData(Strings.Long201, true, "Maxmimum length 200 letters", "Name")]
    public async Task InvalidCommand(
        string name,
        bool receivesEmails,
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

        Func<Task> task = () => ExecuteCommandAsync(new RegisterUserCommand(name, receivesEmails));

        await task.Should()
            .ThrowAsync<InvalidCommandException<RegisterUserCommand>>()
            .WhereHasError(expectedErrorMessage, expectedErrorPropertyPath, because);
    }
}
