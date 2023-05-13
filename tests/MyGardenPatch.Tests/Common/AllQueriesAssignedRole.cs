namespace MyGardenPatch.Tests.Common;

public class QueryTestCaseprovider : IEnumerable<object[]>
{
    public IEnumerator<object[]> GetEnumerator()
    {
        return typeof(IQuery<>).Assembly
            .GetTypes()
            .Where(t => !t.IsAbstract && t.FindInterfaces((@interface, _) => @interface.IsGenericType && @interface.GetGenericTypeDefinition() == typeof(IQuery<>), null).Any())
            .Select(t => new object[] {t})
            .ToList()
            .GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

}

public class AllQueriesAssignedRole
{
    [Theory]
    [ClassData(typeof(QueryTestCaseprovider))]
    public void HasRoleAssigned(Type query)
    {
        query
            .GetCustomAttributes<RoleAttribute>()
            .Should()
            .NotBeEmpty($"{query.FullName} must have a role assigned");
    }
}
