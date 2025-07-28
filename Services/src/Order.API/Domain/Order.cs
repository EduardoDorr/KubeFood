using KubeFood.Core.Entities;
using KubeFood.Core.ValueObjects;

namespace KubeFood.Order.API.Domain;

public class Order : BaseEntity
{
    public int CustomerId { get; private set; }
    public Address DeliveryAddress { get; private set; }
    public List<OrderItem> Items { get; private set; } = [];
    public OrderStatus Status { get; private set; }
    public PaymentType PaymentType { get; private set; }
    public string? TrackingCode { get; private set; }
    public string? FailureReason { get; private set; }
    public decimal TotalPrice => Items.Sum(item => item.Total);

    protected Order() { }

    public Order(
        int customerId,
        Address deliveryAddress,
        List<string> itemIds,
        OrderStatus status,
        PaymentType paymentType)
    {
        CustomerId = customerId;
        DeliveryAddress = deliveryAddress;
        Status = status;
        PaymentType = paymentType;

        AddDomainEvent(new OrderRequested(Id, itemIds));
    }

    public Order(
        int customerId,
        Address deliveryAddress,
        List<OrderItem> items,
        OrderStatus status,
        PaymentType paymentType)
    {
        CustomerId = customerId;
        DeliveryAddress = deliveryAddress;
        Status = status;
        PaymentType = paymentType;

        AddItems(items);

        AddDomainEvent(new OrderRequested(
            Id, items.Select(item => item.ProductId).ToList()));
    }

    public void AddItems(List<OrderItem> items)
    {
        if (items is null || items.Count == 0)
            return;

        Items.AddRange(items);
    }

    public void SetStatus(OrderStatus status)
        => Status = status;

    public void SetTrackingCode(string trackingCode)
        => TrackingCode = trackingCode;

    public void SetFailureReason(string reason)
        => FailureReason = reason;
}