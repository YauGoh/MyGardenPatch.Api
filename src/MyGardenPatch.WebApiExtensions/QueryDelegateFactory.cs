namespace MyGardenPatch.WebApiExtensions;

internal record QueryRoutes(string Route, Delegate Delegate, string[] Roles);

internal class QueryDelegateFactory
{
    internal static IEnumerable<QueryRoutes> ResolveQueryRoutes()
    {
        var getDelegateMethod = typeof(QueryDelegateFactory).GetMethod(nameof(GetDelegate), BindingFlags.Static | BindingFlags.NonPublic)!;

        var queries = Assembly
            .GetAssembly(typeof(IQuery<>))!
            .GetTypes()
            .Where(t => IsQuery(t))
            .Select(t => new { Query = t, Result = GetResultType(t) })
            .Select(a => new QueryRoutes(
                $"/queries/{a.Query.Name}", 
                (Delegate)getDelegateMethod.MakeGenericMethod(a.Query, a.Result).Invoke(null, null)!, 
                GetRoles(a.Query)))
            .ToList();

        return queries;
    }

    internal static Delegate GetDelegate<TQuery, TResult>() where TQuery : IQuery<TResult>
        =>
        async (HttpRequest request, IQueryExecutor queryExecutor, CancellationToken cancellationToken) =>
        {
            var query = await request.ReadFromJsonAsync<TQuery>(cancellationToken);

            return await queryExecutor.HandleAsync(query!, cancellationToken);
        };

    private static string[] GetRoles(Type query)
        => query
            .GetCustomAttributes<RoleAttribute>()
            .Select(a => a.Role)
            .ToArray();

    private static Type GetResultType(Type t)
    {
        return t
            .FindInterfaces((i, _) => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IQuery<>), null)
            .Select(t => t.GetGenericArguments().Single())
            .Single();
    }

    private static bool IsQuery(Type type)
    {
        return !type.IsAbstract && !type.IsInterface &&
            type.FindInterfaces((i, _) => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IQuery<>), null).Any();
    }
}
