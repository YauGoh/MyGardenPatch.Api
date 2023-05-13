namespace MyGardenPatch.WebApi.Tests.TestFixtureExtensions
{
    record RegisteredUser(string FullName, string EmailAddress, string Password);

    internal static class SetupExtensions
    {
        private const string isRegistered = nameof(isRegistered);

        internal static TestFixture WithRegisteredUser(
            this TestFixture fixture,
            string fullName, string emailAddress, string password)
        {
            fixture.SetState(new RegisteredUser(fullName, emailAddress, password));
            fixture.SetState<bool>(isRegistered, false);

            return fixture
                .WithUser(fullName, emailAddress, password)
                .WithLogin(emailAddress, password);
        }

        internal static async Task Setup(this TestFixture testFixture)
        {
            foreach (var user in testFixture.GetCreateUsers())
            {
                var token = await testFixture.StartNewLocalIdenity(user.FullName, user.EmailAddress);
                await testFixture.VerifyLocalIdentityEmailAddress(user.EmailAddress, token, user.Password);
            }
            testFixture.ClearUsers();

            if (testFixture.HasLogin() && string.IsNullOrEmpty(testFixture.GetLoginCookie()))
            {
                var login = testFixture.GetState<Login>();
                await testFixture.SignInLocalIdentity(login.EmailAddress, login.Password);

                if (!testFixture.GetState<bool>(isRegistered) && testFixture.GetState<RegisteredUser>() is not null)
                {
                    await testFixture.RegisterUser(login.EmailAddress, true);

                    testFixture.SetState<bool>(isRegistered, true);
                }
            }
        }

        private static async Task<string> StartNewLocalIdenity(this TestFixture testFixture, string fullName, string emailAddress)
        {
            var tokenExtractor = new EmailTokenExtractor(
                testFixture.MockEmailSender,
                new Regex("<a[^>]*href='.*verification-token=([^'&]*)'>"));

            await testFixture.Sut.Scenario(
                _ =>
                {
                    _.WithRequestHeader("x-api-key", testFixture.Configuration.GetValue<string>("ApiKey"));

                    _.Post
                        .Json(
                            new
                            {
                                Name = fullName,
                                EmailAddress = emailAddress
                            })
                        .ToUrl("/commands/StartNewLocalIdentityCommand");

                    _.StatusCodeShouldBeOk();
                });

            return tokenExtractor.Token;
        }

        private static async Task VerifyLocalIdentityEmailAddress(this TestFixture testFixture, string emailAddress, string verificationToken, string password)
        {
            await testFixture.Sut.Scenario(
               _ =>
               {
                   _.WithRequestHeader("x-api-key", testFixture.Configuration.GetValue<string>("ApiKey"));

                   _.Post
                       .Json(
                           new
                           {
                               EmailAddress = emailAddress,
                               VerificationToken = verificationToken,
                               Password = password,
                               PasswordConfirm = password
                           })
                       .ToUrl("/commands/VerifyLocalIdentityEmailAddressCommand");

                   _.StatusCodeShouldBeOk();
               });
        }

        private static async Task RegisterUser(this TestFixture testFixture, string emailAddress, bool receivesEmails)
        {
            await testFixture.Sut.Scenario(
               _ =>
               {
                   _.WithRequestHeader("Cookie", "my-garden-patch.auth=" + testFixture.GetLoginCookie());

                   var user = testFixture.GetState<RegisteredUser>();

                   _.Post
                       .Json(
                           new
                           {
                               Name = user.FullName,
                               ReceivesEmails = receivesEmails,
                               AcceptsUserAgreement = true
                           })
                       .ToUrl("/commands/RegisterGardenerCommand");

                   _.StatusCodeShouldBeOk();
               });
        }
    }
}
