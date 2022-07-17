using System.Security.Claims;

namespace MyGardenPatch.LocalIdentity;

internal static class ApiKeyAuthentication
{
    internal const string HeaderKey = "x-api-key";
    internal const string Scheme = nameof(ApiKeyAuthentication);
}

internal class ApiKeyAuthenticationOptions : AuthenticationSchemeOptions
{
    public string ApiKey { get; set; } = String.Empty;
}

internal class ApiKeyAuthenticationHandler : AuthenticationHandler<ApiKeyAuthenticationOptions>
{

    public ApiKeyAuthenticationHandler(
        IOptionsMonitor<ApiKeyAuthenticationOptions> options, 
        ILoggerFactory logger, 
        UrlEncoder encoder, 
        ISystemClock clock) : base(options, logger, encoder, clock)
    {
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (!Request.Headers.TryGetValue("x-api-key", out var apiKey) || string.IsNullOrEmpty(apiKey))
            return Task.FromResult(AuthenticateResult.NoResult());

        if (Options.ApiKey != apiKey) return Task.FromResult(AuthenticateResult.Fail("Invalid api key"));

       return Task.FromResult(AuthenticateResult.Success(GetAuthenticationTicket()));
    }

    private AuthenticationTicket GetAuthenticationTicket()
    {
        var claims = new[] {
            new Claim(ClaimTypes.Name, "Api"),
            new Claim(ClaimTypes.NameIdentifier, "Api"),
            new Claim(ClaimTypes.Role, "Api"),
        };

        var principal = new ClaimsPrincipal(
            new ClaimsIdentity(
                claims,
                ApiKeyAuthentication.Scheme));

        return new AuthenticationTicket(principal, ApiKeyAuthentication.Scheme);
    }
}
