using MyGardenPatch.Users;

namespace MyGardenPatch.Common;

public interface ICurrentUserProvider
{
    UserId? UserId { get; }

    string? EmailAddress { get; }

    string? Name { get; }
}
