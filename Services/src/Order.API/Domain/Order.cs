using KubeFood.Core.Entities;
using KubeFood.Core.ValueObjects;
using KubeFood.Order.API.Domain.Events;

namespace KubeFood.Order.API.Domain;

public class Order : BaseEntity<int>
{
    public Guid UniqueId { get; private set; }
    public int CustomerId { get; private set; }
    public Address DeliveryAddress { get; private set; }
    public List<OrderItem> Items { get; private set; } = [];
    public OrderStatus Status { get; private set; }
    public PaymentType PaymentType { get; private set; }
    public string? TrackingCode { get; private set; }
    public string? FailureReason { get; private set; }
    public decimal TotalPrice => Items.Sum(item => item.TotalPrice);

    public virtual List<OrderStatusHistory> StatusHistory { get; private set; } = [];

    protected Order()
    {
        UniqueId = Guid.NewGuid();
    }

    public Order(
        int customerId,
        Address deliveryAddress,
        List<(string, int)> items,
        PaymentType paymentType) : this()
    {
        CustomerId = customerId;
        DeliveryAddress = deliveryAddress;
        Status = OrderStatus.Pending;
        PaymentType = paymentType;
        StatusHistory.Add(new(
            Id,
            OrderStatus.Pending));

        AddDomainEvent(new OrderRequestedEvent(
            UniqueId,
            items.Select(i => new OrderRequestedItemEvent(i.Item1, i.Item2)).ToList()));
    }

    public void AddItems(List<OrderItem> items)
    {
        if (items is null || items.Count == 0)
            return;

        Items.AddRange(items);
    }

    public void SetDeliveryAddress(Address deliveryAddress)
        => DeliveryAddress = deliveryAddress;

    public void SetStatus(OrderStatus status)
        => Status = status;

    public void SetTrackingCode(string trackingCode)
        => TrackingCode = trackingCode;

    public void SetFailureReason(string reason)
        => FailureReason = reason;
}