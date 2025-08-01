using KubeFood.Core.Entities;

namespace KubeFood.Order.API.Domain;

public class OrderItem : BaseEntity
{
    public int OrderId { get; private set; }
    public string ProductId { get; private set; }
    public string Name { get; private set; }
    public decimal Price { get; private set; }
    public int Quantity { get; private set; }
    public decimal TotalPrice => Price * Quantity;

    public virtual Order Order { get; private set; }

    protected OrderItem() { }

    public OrderItem(int orderId, string productId, string name, decimal price, int quantity)
    {
        OrderId = orderId;
        ProductId = productId;
        Name = name;
        Price = price;
        Quantity = quantity;
    }
}