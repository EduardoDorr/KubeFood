namespace KubeFood.Order.API.Application.OrderSaga.OrderValidated;

public sealed record OrderValidatedEvent(
    Guid OrderUniqueId,
    bool Valid,
    List<OrderValidatedItemEvent> OrderItems,
    List<string>? InvalidOrderItems = null);

public sealed record OrderValidatedItemEvent(
    string ProductId,
    string Name,
    decimal Price,
    int Quantity);