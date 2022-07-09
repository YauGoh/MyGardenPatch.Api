namespace MyGardenPatch.Events;

public interface IDomainEventBus
{
    Task PublishAsync<TDomainEvent>(TDomainEvent domainEvent, CancellationToken cancellationToken) where TDomainEvent : IDomainEvent;
}
