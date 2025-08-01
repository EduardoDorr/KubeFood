using System.Collections.Immutable;

using KubeFood.Core.ValueObjects;
using KubeFood.Order.API.Domain;

namespace KubeFood.Order.API.Application.Models;

public sealed record OrderViewModel(
    int Id,
    int CustomerId,
    IReadOnlyList<OrderItemViewModel> Items,
    decimal TotalPrice,
    OrderStatus Status,
    PaymentType PaymentType,
    Address DeliveryAddress,
    string? TrackingCode,
    string? FailureReason);

public static class OrderViewModelExtensions
{
    public static OrderViewModel? ToModel(this Domain.Order order)
        => order is null
            ? null
            : new(
                order.Id,
                order.CustomerId,
                order.Items.ToModel().ToImmutableList(),
                order.TotalPrice,
                order.Status,
                order.PaymentType,
                order.DeliveryAddress,
                order.TrackingCode,
                order.FailureReason);

    public static IEnumerable<OrderViewModel> ToModel(this IEnumerable<Domain.Order> orders)
        => orders is null
            ? []
            : orders.Select(p => p.ToModel()!);
}