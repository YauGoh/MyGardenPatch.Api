namespace MyGardenPatch.Common;

[AttributeUsage(AttributeTargets.Class)]
public class RoleAttribute : Attribute
{
    public RoleAttribute(string role)
    {
        Role = role;
    }

    public string Role { get; }
}

public static class WellKnownRoles
{
    public const string Gardener = "Gardener";

    public const string Api = "Api";
}