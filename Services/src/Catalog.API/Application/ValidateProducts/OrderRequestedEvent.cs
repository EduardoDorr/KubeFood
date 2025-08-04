namespace KubeFood.Catalog.API.Application.ValidateProducts;

public sealed record OrderRequestedEvent(Guid OrderUniqueId, List<OrderRequestedItemEvent> OrderItems);
public sealed record OrderRequestedItemEvent(string Id, int Quantity);