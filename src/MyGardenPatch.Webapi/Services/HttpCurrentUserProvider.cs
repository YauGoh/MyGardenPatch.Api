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

    public HttpCurrentUserProvider(IHttpContextAccessor httpContextAccessor, IRepository<User, UserId> users)
    {
        _httpContextAccessor = httpContextAccessor;
        _users = users;
    }

    public UserId? CurrentUserId => _userId ?? (_userId = ResolveUserId());

    public string? CurrentEmailAddress => _emailAddress ?? (_emailAddress = _httpContextAccessor.HttpContext!.User.Identities!
            .SelectMany(i => i.Claims)
            .Where(c => c.Type == ClaimTypes.Email)
            .Select(c => c.Value)
            .FirstOrDefault());

    private UserId? ResolveUserId()
    {
        if (CurrentEmailAddress is null) return null;

        var user = _users.GetByExpressionAsync(u => u.EmailAddress == CurrentEmailAddress).Result;

        return user?.Id;
    }
}