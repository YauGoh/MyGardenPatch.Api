using MyGardenPatch.Users;

namespace MyGardenPatch.Common;

public interface ICurrentUserProvider
{
    UserId? CurrentUserId { get; }

    string? CurrentEmailAddress { get; }
}
