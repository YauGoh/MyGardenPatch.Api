namespace MyGardenPatch.WebApi.Tests;

internal class OrderedByDependantTests : ITestCaseOrderer
{
    public IEnumerable<TTestCase> OrderTestCases<TTestCase>(IEnumerable<TTestCase> testCases) where TTestCase : ITestCase
    {
        return testCases.OrderBy(testCase => testCase.TestMethod.Method.GetCustomAttributes(typeof(OrderAttribute)).Cast<ReflectionAttributeInfo>().FirstOrDefault()?.Attribute.As<OrderAttribute>()?.Order);
    }
}

public class OrderAttribute : Attribute
{
    public OrderAttribute(int order)
    {
        Order = order;
    }

    public int Order { get; }
}
