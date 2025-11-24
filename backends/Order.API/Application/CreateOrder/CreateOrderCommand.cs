using KubeFood.Order.API.Application.Models;
using KubeFood.Order.API.Domain;

namespace KubeFood.Order.API.Application.CreateOrder;

public sealed record CreateOrderCommand(
    int CustomerId,
    List<CreateOrderItemCommand> Items,
    PaymentType PaymentType,
    AddressModel DeliveryAddress);

public sealed record CreateOrderItemCommand(
    string Id,
    int Quantity);

public static class CreateOrderCommandExtensions
{
    public static Domain.Order ToOrder(this CreateOrderCommand command)
        => new(
            command.CustomerId,
            command.DeliveryAddress.ToAddress(),
            command.Items.Select(i => (i.Id, i.Quantity)).ToList(),
            command.PaymentType);
}