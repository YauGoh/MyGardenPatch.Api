using MyGardenPatch.GardenBeds;
using System.Net;

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

    private bool useApiKey = true;

    private bool registerUser = false;
    private string registerUserFullname = string.Empty;
    private string validateEmailToken = string.Empty;

    private GardenId gardenId;
    private GardenBedId gardenBedId;


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

        EnsureSeedData(Sut.Services);
    }

    internal IAlbaHost Sut { get; private set; }

    public TestFixture UseApiKey()
    {
        useApiKey = true;

        hasLogin = false;


        return this;
    }

    public TestFixture WithUser(string fullName, string emailAddress, string password)
    {
        createUsers.Add(new CreateUser(fullName, emailAddress, password));

        return this;
    }

    public TestFixture WithRegisteredUser(string fullName, string emailAddress, string password)
    {
        this.registerUser = true;
        this.registerUserFullname = fullName;

        return WithUser(fullName, emailAddress, password)
            .WithLogin(emailAddress, password);
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
        useApiKey = false;

        return this;
    }

    public TestFixture WithApiKey()
    {
        useApiKey = true;

        return this;
    }

    public TestFixture WithGardenId(GardenId gardenId)
    {
        this.gardenId = gardenId;
        return this;
    }

    public GardenId GardenId => gardenId;

    internal TestFixture WithGardenBedId(GardenBedId gardenBedId)
    {
        this.gardenBedId = gardenBedId;
        return this;
    }

    public GardenBedId GardenBedId => gardenBedId;

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
                else if (useApiKey)
                {
                    _.WithRequestHeader("x-api-key", Configuration.GetValue<string>("ApiKey"));
                }

                configure?.Invoke(_);
            });
    }

    public async Task Command(string url, dynamic command = null)
    {
        command = command ?? new { };

        await Scenario(
            _ =>
            {
                _.Post
                    .Json(command)
                    .ToUrl(url);

                _.StatusCodeShouldBeOk();
            });
    }

    public async Task<TResult> Query<TResult>(string url, dynamic? query = null)
    {
        query = query ?? new { };

        var response = await Scenario(
            _ =>
            {
                _.Post
                    .Json(query)
                    .ToUrl(url);

                _.StatusCodeShouldBeOk();
            });

        var result = response
            .ReadAsJson<TResult>()!;

        return result;
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

            if (registerUser)
            {
                await RegisterUser(loginEmailAddress, true);

                registerUser = false;
            }
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

    private async Task RegisterUser(string emailAddress, bool receivesEmails)
    {
        await Sut.Scenario(
           _ =>
           {
               _.WithRequestHeader("Cookie", "my-garden-patch.auth=" + loginCookie);

               _.Post
                   .Json(
                       new
                       {
                           Name = registerUserFullname,
                           EmailAddress = emailAddress,
                           ReceivesEmails = receivesEmails
                       })
                   .ToUrl("/commands/RegisterUserCommand");

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

    private void EnsureSeedData(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();

        var localIdentityDbContext = scope.ServiceProvider.GetRequiredService<LocalIdentityDbContext>();
        localIdentityDbContext.Database.EnsureCreated();

       
        var myGardenDbContext = scope.ServiceProvider.GetRequiredService<MyGardenPatchDbContext>();
        myGardenDbContext.Database.EnsureCreated();
    }

    record CreateUser(string FullName, string EmailAddress, string Password);
}
