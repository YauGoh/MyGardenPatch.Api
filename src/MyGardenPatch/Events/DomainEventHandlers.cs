namespace MyGardenPatch.Events;

internal record DomainHandlerInfo(Type Handler, Type Interface);

internal static class DomainEventHandlers
{
    internal static IEnumerable<DomainHandlerInfo> DiscoverAll()
    {
        var genericDomainHandler = typeof(IDomainEventHandler<>);

        return Assembly
            .GetExecutingAssembly()
            .GetTypes()
            .Where(t => !t.IsAbstract && IsDomainEventHandler(t))
            .Select(t => new DomainHandlerInfo(
                t, 
                genericDomainHandler.MakeGenericType(GetDomainEventType(t))))
            .ToList();

    }

    private static bool IsDomainEventHandler(Type t)
    {
        return t
            .GetInterfaces()
            .Any(t => t.IsGenericType &&
                      t.GetGenericTypeDefinition() == typeof(IDomainEventHandler<>));
    }

    private static Type GetDomainEventType(Type t)
    {
        return t
            .GetInterfaces()
            .Where(t => t.IsGenericType &&
                      t.GetGenericTypeDefinition() == typeof(IDomainEventHandler<>))
            .Select(t => t.GetGenericArguments().First())
            .First();
    }
}
