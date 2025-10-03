using KubeFood.Core.Events;

namespace KubeFood.Delivery.Worker.Domain;

public sealed record OrderStatusChangedEvent(Guid Id, OrderStatus Status) : IEvent;