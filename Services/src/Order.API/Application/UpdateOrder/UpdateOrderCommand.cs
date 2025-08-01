using KubeFood.Core.ValueObjects;

namespace KubeFood.Order.API.Application.UpdateOrder;

public sealed record UpdateOrderCommand(
    int Id,
    Address DeliveryAddress);