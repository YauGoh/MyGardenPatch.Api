namespace MyGardenPatch.Tests.Assertions;

internal static class StringAssertions
{
    public static bool Like(this string str, Regex pattern) => pattern.IsMatch(str);
}