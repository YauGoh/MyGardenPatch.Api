namespace MyGardenPatch.LocalIdentity;

public static class ServiceCollectionExtensions
{
    internal const string ApplicationCookieName = "my-garden-patch.auth";

    public static IServiceCollection AddLocalIdentity(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddTransient<ILocalIdentityManager, LocalIdentityMananger>();

        services
            .AddDbContext<LocalIdentityDbContext>(
                options => options.UseSqlServer(configuration.GetConnectionString("LocalIdentity")));

        services
            .AddIdentityCore<LocalIdentityUser>(options =>
            {
                options.User.RequireUniqueEmail = true;
                options.Password.RequireDigit = false;
                options.Password.RequiredLength = 1;
                options.Password.RequiredUniqueChars = 1;
                options.Password.RequireLowercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;

                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                options.Lockout.MaxFailedAccessAttempts = 5;

                options.SignIn.RequireConfirmedEmail = true;
            })
            .AddRoles<LocalIdentityRole>()
            .AddSignInManager<SignInManager<LocalIdentityUser>>()
            .AddEntityFrameworkStores<LocalIdentityDbContext>()
            .AddDefaultTokenProviders();

        services
            .AddAuthentication(
                options =>
                {
                    options.DefaultAuthenticateScheme = "AuthenticationSchemaSelect";
                    options.DefaultChallengeScheme = "AuthenticationSchemaSelect";
                    options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
                })
            .AddPolicyScheme(
                "AuthenticationSchemaSelect", 
                "Selects between Apikey or Cookie based authentication", 
                options =>
                {
                    options.ForwardDefaultSelector = context =>
                    {
                        var hasApiKey = context.Request.Headers[ApiKeyAuthentication.HeaderKey].Any();

                        if (hasApiKey)
                            return ApiKeyAuthentication.Scheme;

                        return IdentityConstants.ApplicationScheme;
                    };
                })
            .AddScheme<ApiKeyAuthenticationOptions, ApiKeyAuthenticationHandler>(
                ApiKeyAuthentication.Scheme, 
                options => options.ApiKey = configuration.GetValue<string>("ApiKey"))
            .AddIdentityCookies(
                options => options.ApplicationCookie.Configure(ac =>
                {
                    ac.Cookie.Name = ApplicationCookieName;


                    ac.Events.OnRedirectToLogin = context =>
                    {
                        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        return Task.CompletedTask;
                    };
                }));

        return services;
    }
}