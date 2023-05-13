namespace MyGardenPatch.Tests.Common;

public class CommandTestCaseprovider : IEnumerable<object[]>
{
    public IEnumerator<object[]> GetEnumerator()
    {
        return typeof(ICommand).Assembly
            .GetTypes()
            .Where(t => !t.IsAbstract && t.IsAssignableTo(typeof(ICommand)))
            .Select(t => new object[] { t })
            .ToList()
            .GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

}

public class AllCommandsAssignedRole
{
    [Theory]
    [ClassData(typeof(CommandTestCaseprovider))]
    public void HasRoleAssigned(Type command)
    {
        command
            .GetCustomAttributes<RoleAttribute>()
            .Should()
            .NotBeEmpty($"{command.FullName} must have a role assigned");
    }
}
