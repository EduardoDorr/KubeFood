using KubeFood.Core.Entities;

namespace KubeFood.Order.API.Domain;

public class OrderItem : BaseEntity
{
    public string ProductId { get; private set; }
    public string Name { get; private set; }
    public decimal Price { get; private set; }
    public int Quantity { get; private set; }
    public decimal Total => Price * Quantity;

    public OrderItem(string productId, string name, decimal price, int quantity)
    {
        ProductId = productId;
        Name = name;
        Price = price;
        Quantity = quantity;
    }
}