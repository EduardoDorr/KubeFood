namespace KubeFood.Order.API.Application.Models;

public sealed record OrderItemViewModel(
    string ProductId,
    string Name,
    int Quantity,
    decimal Price,
    decimal TotalPrice);

public static class OrderItemViewModelExtensions
{
    public static OrderItemViewModel? ToModel(this Domain.OrderItem orderItem)
        => orderItem is null
            ? null
            : new(
                orderItem.ProductId,
                orderItem.Name,
                orderItem.Quantity,
                orderItem.Price,
                orderItem.TotalPrice);

    public static IEnumerable<OrderItemViewModel> ToModel(this IEnumerable<Domain.OrderItem> orderItems)
        => orderItems is null
            ? []
            : orderItems.Select(o => o.ToModel()!);
}