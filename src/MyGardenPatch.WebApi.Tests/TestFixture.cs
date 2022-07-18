﻿using System.Net;

namespace MyGardenPatch.WebApi.Tests;

public class TestFixture
{
    public Mock<IEmailSender> MockEmailSender { get; }
    private IConfiguration Configuration { get; }

    private List<CreateUser> createUsers = new List<CreateUser>();

    private string loginCookie = string.Empty;

    private bool hasLogin = false;
    private string loginEmailAddress = string.Empty;
    private string loginPassword = string.Empty;

    private string userName = string.Empty;


    public TestFixture()
    {
        MockEmailSender = new Mock<IEmailSender>();

        Sut = AlbaHost.For<global::Program>(x =>
        {
            x.ConfigureServices((context, services) =>
            {
                services.AddTransient<IEmailSender>(s => MockEmailSender.Object);

                var databaseName = Guid.NewGuid().ToString();

                ReplaceWithInmemory<LocalIdentityDbContext>(services, databaseName);
                ReplaceWithInmemory<MyGardenPatchDbContext>(services, databaseName);
            });
        }).Result;

        Configuration = Sut.Services.GetRequiredService<IConfiguration>();
    }

    internal IAlbaHost Sut { get; private set; }

    public TestFixture WithUser(string fullName, string emailAddress, string password)
    {
        createUsers.Add(new CreateUser(fullName, emailAddress, password));

        return this;
    }

    public TestFixture WithLogin(string emailAddress, string password)
    {
        loginEmailAddress = emailAddress;
        loginPassword = password;

        hasLogin = true;
        loginCookie = string.Empty;

        return this;
    }

    public TestFixture WithNoLogin()
    {
        hasLogin = false;
        loginCookie = string.Empty;

        return this;
    }

    internal TestFixture WithPasswordChangeRequest()
    {


        return this;
    }

    public async Task<IScenarioResult> Scenario(Action<Scenario> configure)
    {
        await Setup();

        return await Sut.Scenario(
            _ =>
            {
                if (hasLogin && !string.IsNullOrEmpty(loginCookie))
                {
                    _.WithRequestHeader("Cookie", "my-garden-patch.auth=" + loginCookie);
                }
                else
                {
                    _.WithRequestHeader("x-api-key", Configuration.GetValue<string>("ApiKey"));
                }

                configure?.Invoke(_);
            });
    }

    

    public async Task Setup()
    {
        foreach (var user in createUsers)
        {
            var token = await StartNewLocalIdenity(user.FullName, user.EmailAddress);
            await VerifyLocalIdentityEmailAddress(user.EmailAddress, token, user.Password);
        }
        createUsers.Clear();

        if (hasLogin && string.IsNullOrEmpty(loginCookie))
        {
            await SignInLocalIdentity(loginEmailAddress, loginPassword);
        }
    }

    private async Task SignInLocalIdentity(string emailAddress, string password)
    {
        var result = await Sut.Scenario(
           _ =>
           {
               _.WithRequestHeader("x-api-key", Configuration.GetValue<string>("ApiKey"));

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
        loginCookie = cookieContainer.GetCookies(new Uri("https://localhost"))["my-garden-patch.auth"]!.Value;
    }

    private async Task<string> StartNewLocalIdenity(string fullName, string emailAddress)
    {
        var tokenExtractor = new EmailTokenExtractor(
            MockEmailSender, 
            new Regex("<a[^>]*href='.*verificationToken=([^'&]*)'>"));

        await Sut.Scenario(
            _ =>
            {
                _.WithRequestHeader("x-api-key", Configuration.GetValue<string>("ApiKey"));
                
                _.Post
                    .Json(
                        new
                        {
                            FullName = fullName,
                            EmailAddress = emailAddress
                        })
                    .ToUrl("/commands/StartNewLocalIdentityCommand");

                _.StatusCodeShouldBeOk();
            });

        return tokenExtractor.Token;
    }

    private async Task VerifyLocalIdentityEmailAddress(string emailAddress, string verificationToken, string password)
    {
        await Sut.Scenario(
           _ =>
           {
               _.WithRequestHeader("x-api-key", Configuration.GetValue<string>("ApiKey"));

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

    private void ReplaceWithInmemory<TDbContext>(IServiceCollection services, string databaseName) where TDbContext : DbContext
    {

        var serviceDescriptions = services.Where(s => s.ServiceType == typeof(TDbContext) ||
                                             //s.ServiceType == typeof(DbContextOptions) ||
                                             s.ServiceType == typeof(DbContextOptions<TDbContext>)).ToList();
        foreach (var serviceDescription in serviceDescriptions)
        {
            services.Remove(serviceDescription);
        }

        services.AddDbContext<TDbContext>(options => options.UseInMemoryDatabase(databaseName));

    }

    record CreateUser(string FullName, string EmailAddress, string Password);
}
