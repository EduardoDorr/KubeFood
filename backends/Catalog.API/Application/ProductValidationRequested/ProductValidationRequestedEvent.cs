using KubeFood.Core.Events;

namespace KubeFood.Catalog.API.Application.ProductValidationRequested;

public sealed record ProductValidationRequestedEvent(
    Guid Id,
    List<ProductValidationRequestedItemEvent> Items)
    : IEvent;

public sealed record ProductValidationRequestedItemEvent(
    string Id,
    int Quantity);