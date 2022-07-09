namespace MyGardenPatch.Tests.Users.Commands;

public class RegisterUserCommandTests : TestBase
{
    [Fact]
    public async Task RegisterNewUsers()
    {
        var date = new DateTime(2022, 1, 1);
        await ExecuteCommandAsync(new RegisterUserCommand(UserTestData.UnregisteredUser.Name, UserTestData.UnregisteredUser.EmailAddress, date, true));

        var user = await GetAsync<User, UserId>(u => u.EmailAddress == UserTestData.UnregisteredUser.EmailAddress);

        user!.Name.Should().Be(UserTestData.UnregisteredUser.Name);
        user.EmailAddress.Should().Be(UserTestData.UnregisteredUser.EmailAddress);
        user.RegisteredAt.Should().Be(date);
        user.ReceivesEmail.Should().BeTrue();
    }

    [Fact]
    public async Task RegisterNewUsersRaisesNewUserRegisteredEvent()
    {
        var date = new DateTime(2022, 1, 1);
        await ExecuteCommandAsync(new RegisterUserCommand(UserTestData.UnregisteredUser.Name, UserTestData.UnregisteredUser.EmailAddress, date, true));

        MockDomainEventBus.Verify(
            _ => _.PublishAsync(
                It.Is<NewUserRegistered>(
                    e => e.RegisteredAt == date),
                default));
    }

    [Theory]
    [InlineData("", "john.doe@email.com.au", "2022/1/1", true, "Name is required", "Name")]
    [InlineData(Strings.Long201, "john.doe@email.com.au", "2022/1/1", true, "Maxmimum length 200 letters", "Name")]
    [InlineData("John Doe", "", "2022/1/1", true, "Email address is required", "EmailAddress")]
    [InlineData("John Doe", "not an email address", "2022/1/1", true, "Invalid Email address", "EmailAddress")]
    [InlineData("John Doe", Strings.LongEmail201, "2022/1/1", true, "Maxmimum length 200 letters", "EmailAddress")]
    [InlineData("John Doe", UserTestData.PeterParkerEmailAddress, "2022/1/1", true, "User with email address is already registered", "EmailAddress", "User is already registered")]
    public async Task InvalidCommand(
        string name,
        string emailAddress,
        DateTime registeredAt,
        bool receivesEmails,
        string expectedErrorMessage,
        string expectedErrorPropertyPath,
        string because = "")
    {
        SeedWith(UserTestData.PeterParker);

        Func<Task> task = () => ExecuteCommandAsync(new RegisterUserCommand(name, emailAddress, registeredAt, receivesEmails));

        await task.Should()
            .ThrowAsync<InvalidCommandException<RegisterUserCommand>>()
            .WhereHasError(expectedErrorMessage, expectedErrorPropertyPath, because);
    }
}
