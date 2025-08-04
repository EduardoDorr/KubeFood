using KubeFood.Core.Entities;

namespace KubeFood.Order.API.Domain;

public class OrderStatusHistory : BaseEntity<int>
{
    public int OrderId { get; private set; }
    public OrderStatus Status { get; private set; }
    public string? FailureReason { get; private set; }

    public virtual Order Order { get; private set; }

    protected OrderStatusHistory() { }

    public OrderStatusHistory(
        int orderId,
        OrderStatus orderStatus,
        string? failureReason = null)
    {
        OrderId = orderId;
        Status = orderStatus;
        FailureReason = failureReason;
    }
}