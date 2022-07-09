using Microsoft.AspNetCore.Identity;

namespace MyGardenPatch.LocalIdentity;

internal class LocalIdentityUser : IdentityUser
{
    public string FullName { get; set; } = string.Empty;
}
