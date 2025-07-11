using MediatR;

namespace Core.DomainEvents;

public interface IDomainEventHandler<in TDomainEvent>
    : INotificationHandler<TDomainEvent> where TDomainEvent : IDomainEvent
{
}