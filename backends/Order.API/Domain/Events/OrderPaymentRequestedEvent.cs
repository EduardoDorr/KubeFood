using KubeFood.Core.Events;

namespace KubeFood.Order.API.Domain.Events;

public sealed record OrderPaymentRequestedEvent(
    Guid Id)
    : IDomainEvent;