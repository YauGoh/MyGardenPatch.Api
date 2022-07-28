namespace MyGardenPatch.WebApi.Tests;

[TestCaseOrderer("MyGardenPatch.WebApi.Tests.OrderedByDependantTests", "MyGardenPatch.WebApi.Tests")]
public class LocalIdentityScenarios : IClassFixture<TestFixture>
{
    private readonly TestFixture _fixture;

    public LocalIdentityScenarios(TestFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact, Order(0)]
    public async Task NotAuthorised()
    {
        var response = await _fixture
            .WithNoLogin()
            .Scenario(
                _ =>
                {
                    _.Post
                        .Json(new { })
                        .ToUrl("/queries/GetUserRegistrationStatusQuery");

                    _.StatusCodeShouldBe(HttpStatusCode.Unauthorized);
                });
    }

    [Fact, Order(1)]
    public async Task LoginWithNewIdentity()
    {
        var response = await _fixture
            .WithApiKey()
            .WithUser("Peter Parker", "peter.parker@email.com", "password1234!")
            .WithLogin("peter.parker@email.com", "password1234!")
            .Scenario(
                _ => 
                {
                    _.Post
                        .Json(new {})
                        .ToUrl("/queries/GetUserRegistrationStatusQuery");

                    _.StatusCodeShouldBeOk();
                });

        var content = response.ReadAsJson<UserRegistrationStatus>()!;

        content.Status.Should().Be(RegistrationStatus.NotRegistered);
    }

    [Fact, Order(2)]
    public async Task RegisterNewUser()
    {
        var response = await _fixture
            .Scenario(
                _ =>
                {
                    _.Post
                        .Json(new { 
                            Name = "Peter",
                            ReceivesEmails = true
                        })
                        .ToUrl("/commands/RegisterUserCommand");

                    _.StatusCodeShouldBeOk();
                });
    }

    [Fact, Order(3)]
    public async Task DontAllowMultipleRegistrations()
    {
        var response = await _fixture
            .Scenario(
                _ =>
                {
                    _.Post
                        .Json(new
                        {
                            Name = "Peter",
                            ReceivesEmails = false
                        })
                        .ToUrl("/commands/RegisterUserCommand");

                    _.StatusCodeShouldBe(400);
                });
    }

    [Fact, Order(4)]
    public async Task RequestChangePassword()
    {
        var emailTokenExtractor = new EmailTokenExtractor(
            _fixture.MockEmailSender, 
            new Regex("<a[^>]*href='.*passwordToken=([^'&]*)'>"));

        var response = await _fixture
           .Scenario(
                _ =>
                {
                    _.Post
                        .Json(new { })
                        .ToUrl("/commands/RequestChangePasswordLocalIdentityCommand");

                    _.StatusCodeShouldBeOk();
                });

        var token = emailTokenExtractor.Token;

        response = await _fixture
            .WithApiKey()
            .Scenario(
                _ =>
                {
                    _.Post
                        .Json(new 
                        {
                            EmailAddress = "peter.parker@email.com",
                            PasswordResetToken = token,
                            Password = "NEW PASSWORD",
                            PasswordConfirm = "NEW PASSWORD"
                        })
                        .ToUrl("/commands/ResetPasswordLocalIdentityCommand");

                    _.StatusCodeShouldBeOk();
                });
    }

    [Fact, Order(5)]
    public async Task LoginWithNewPassowrd()
    {
        var response = await _fixture
            .WithLogin("peter.parker@email.com", "NEW PASSWORD")
            .Scenario(
                _ =>
                {
                    _.Post
                        .Json(new { })
                        .ToUrl("/queries/GetUserRegistrationStatusQuery");

                    _.StatusCodeShouldBeOk();
                });

        var content = response.ReadAsJson<UserRegistrationStatus>()!;

        content.Status.Should().Be(RegistrationStatus.Registered);
    }

    [Fact, Order(6)]
    public async Task ForgotPassword()
    {
        var emailTokenExtractor = new EmailTokenExtractor(
            _fixture.MockEmailSender,
            new Regex("<a[^>]*href='.*passwordToken=([^'&]*)'>"));

        var response = await _fixture
           .WithApiKey()
           .Scenario(
                _ =>
                {
                    _.Post
                        .Json(new 
                        { 
                            EmailAddress = "peter.parker@email.com"
                        })
                        .ToUrl("/commands/RequestForgotPasswordLocalIdentityCommand");

                    _.StatusCodeShouldBeOk();
                });

        var token = emailTokenExtractor.Token;

        response = await _fixture
            .WithApiKey()
            .Scenario(
                _ =>
                {
                    _.Post
                        .Json(new
                        {
                            EmailAddress = "peter.parker@email.com",
                            PasswordResetToken = token,
                            Password = "AFTER FORGOTTEN PASSWORD CHANGE",
                            PasswordConfirm = "AFTER FORGOTTEN PASSWORD CHANGE"
                        })
                        .ToUrl("/commands/ResetPasswordLocalIdentityCommand");

                    _.StatusCodeShouldBeOk();
                });
    }

    [Fact, Order(7)]
    public async Task LoginWithNewPassowrdAfterForgottenPasswordChange()
    {
        var response = await _fixture
            .WithLogin("peter.parker@email.com", "AFTER FORGOTTEN PASSWORD CHANGE")
            .Scenario(
                _ =>
                {
                    _.Post
                        .Json(new { })
                        .ToUrl("/queries/GetUserRegistrationStatusQuery");

                    _.StatusCodeShouldBeOk();
                });

        var content = response.ReadAsJson<UserRegistrationStatus>()!;

        content.Status.Should().Be(RegistrationStatus.Registered);
    }
}
