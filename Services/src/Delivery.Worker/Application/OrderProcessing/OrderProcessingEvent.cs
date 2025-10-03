using KubeFood.Core.Events;

namespace KubeFood.Delivery.Worker.Application.OrderProcessing;

public sealed record OrderProcessingEvent(Guid Id) : IEvent;