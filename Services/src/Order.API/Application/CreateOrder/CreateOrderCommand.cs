using KubeFood.Order.API.Application.Models;
using KubeFood.Order.API.Domain;

namespace KubeFood.Order.API.Application.CreateOrder;

public sealed record CreateOrderCommand(
    int CustomerId,
    List<string> ItemIds,
    PaymentType PaymentType,
    AddressModel DeliveryAddress);

public static class CreateOrderCommandExtensions
{
    public static Domain.Order ToOrder(this CreateOrderCommand command)
        => new(
            command.CustomerId,
            command.DeliveryAddress.ToAddress(),
            command.ItemIds,
            command.PaymentType);
}