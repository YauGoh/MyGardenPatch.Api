namespace MyGardenPatch.WebApi.Tests.TestFixtureExtensions
{
    internal record Login(string EmailAddress, string Password);

    internal static class LoginExtensions
    {
        internal const string useApiKey = nameof(useApiKey);
        internal const string hasLogin = nameof(hasLogin);
        internal const string loginCookie = nameof(loginCookie);

        internal static TestFixture WithApiKey(this TestFixture testFixture)
        {
            testFixture.SetState(useApiKey, true);
            testFixture.SetState(hasLogin, false);
            testFixture.SetState(loginCookie, string.Empty);

            return testFixture;
        }

        internal static TestFixture WithLogin(this TestFixture testFixture, string emailAddress, string password)
        {
            testFixture.SetState(new Login(emailAddress, password));

            testFixture.SetState(hasLogin, true);
            testFixture.SetState(loginCookie, string.Empty);

            return testFixture;
        }


        internal static TestFixture WithNoLogin(this TestFixture testFixture)
        {
            testFixture.SetState(useApiKey, false);
            testFixture.SetState(hasLogin, false);
            testFixture.SetState(loginCookie, string.Empty);

            return testFixture;
        }

        internal static bool UseApiKey(this TestFixture testFixture)
            => testFixture.GetState<bool>(useApiKey);

        internal static bool HasLogin(this TestFixture testFixture)
            => testFixture.GetState<bool>(hasLogin);

        internal static string GetLoginCookie(this TestFixture testFixture)
            => testFixture.GetState<string>(loginCookie);

        internal static void SetLoginCookie(this TestFixture testFixture, string cookie)
            => testFixture.SetState<string>(loginCookie, cookie);

        internal static async Task SignInLocalIdentity(
            this TestFixture testFixture, 
            string emailAddress, 
            string password)
        {
            var result = await testFixture.Sut.Scenario(
               _ =>
               {
                   _.WithRequestHeader("x-api-key", testFixture.Configuration.GetValue<string>("ApiKey"));

                   _.Post
                       .Json(
                           new
                           {
                               EmailAddress = emailAddress,
                               Password = password,
                           })
                       .ToUrl("/commands/SignInLocalIdentityCommand");

                   _.StatusCodeShouldBeOk();
               });

            var cookies = result.Context.Response.Headers["Set-Cookie"];
            var cookieContainer = new CookieContainer();
            cookieContainer.SetCookies(new Uri("https://localhost"), cookies);

            testFixture.SetLoginCookie(cookieContainer.GetCookies(new Uri("https://localhost"))["my-garden-patch.auth"]!.Value);
        }

        internal static async Task<IScenarioResult> WithAuth(
            this TestFixture testFixture, 
            Action<Scenario> configure)
        {
            return await testFixture.Sut.Scenario(
            _ =>
            {
                if (testFixture.HasLogin() && !string.IsNullOrEmpty(testFixture.GetLoginCookie()))
                {
                    _.WithRequestHeader("Cookie", "my-garden-patch.auth=" + testFixture.GetLoginCookie());
                }
                else if (testFixture.UseApiKey())
                {
                    _.WithRequestHeader("x-api-key", testFixture.Configuration.GetValue<string>("ApiKey"));
                }

                configure?.Invoke(_);
            });
        }
    }
}
