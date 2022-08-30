using MyGardenPatch.Aggregates;
using MyGardenPatch.Common;
using MyGardenPatch.Users;
using System.Security.Claims;

namespace MyGardenPatch.Webapi.Services;

internal class HttpCurrentUserProvider : ICurrentUserProvider
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IRepository<User, UserId> _users;

    UserId? _userId = null;
    string? _emailAddress = null;
    string? _name = null;

    public HttpCurrentUserProvider(IHttpContextAccessor httpContextAccessor, IRepository<User, UserId> users)
    {
        _httpContextAccessor = httpContextAccessor;
        _users = users;
    }

    public UserId? UserId => _userId ??= ResolveUserId();

    public string? EmailAddress => _emailAddress ??= _httpContextAccessor.HttpContext!.User.Identities!
            .SelectMany(i => i.Claims)
            .Where(c => c.Type == ClaimTypes.Email)
            .Select(c => c.Value)
            .LastOrDefault();

    public string? Name => _name ??= _httpContextAccessor.HttpContext!.User.Identities!
            .SelectMany(i => i.Claims)
            .Where(c => c.Type == ClaimTypes.Name)
            .Select(c => c.Value)
            .LastOrDefault();

    private UserId? ResolveUserId()
    {
        if (EmailAddress is null) return null;

        var user = _users.GetByExpressionAsync(u => u.EmailAddress == EmailAddress).Result;

        return user?.Id;
    }
}