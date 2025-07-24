using Core.Entities;

namespace Inventory.API.Domain;

public class Order : BaseEntity
{
    public int CustomerId { get; private set; }
}