using KubeFood.Core.Events;
using KubeFood.Order.API.Domain;

namespace KubeFood.Order.API.Application.OrderOrchestration.OrderStatusChanged;

public sealed record OrderStatusChangedEvent(Guid Id, OrderStatus Status) : IEvent;