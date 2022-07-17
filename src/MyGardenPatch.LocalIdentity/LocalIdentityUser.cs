namespace MyGardenPatch.LocalIdentity;

internal class LocalIdentityUser : IdentityUser<Guid>
{
    public string FullName { get; set; } = string.Empty;

    public string ApiKey { get; set; } = string.Empty;
}

internal class LocalIdentityRole : IdentityRole<Guid>
{

}