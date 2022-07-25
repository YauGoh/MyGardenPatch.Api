namespace MyGardenPatch.Events;

internal class InMemoryDomainEventBus : IDomainEventBus
{
    private readonly IServiceProvider _serviceProvider;

    public InMemoryDomainEventBus(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task PublishAsync<TDomainEvent>(TDomainEvent domainEvent, CancellationToken cancellationToken) where TDomainEvent : IDomainEvent
    {
        var handlers = _serviceProvider.GetServices<IDomainEventHandler<TDomainEvent>>();

        var tasks = handlers.Select(h => h.HandleAsync(domainEvent, cancellationToken));

        await Task.WhenAll(tasks);
    }
}
