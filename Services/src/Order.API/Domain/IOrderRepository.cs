using KubeFood.Core.Models.Pagination;

using MongoDB.Bson;

namespace KubeFood.Order.API.Domain;

public interface IOrderRepository
{
    Task<PaginationResult<Order>> GetAllAsync(int page = 1, int pageSize = 10, CancellationToken cancellationToken = default);
    Task<PaginationResult<Order>> GetByCustomerAsync(Guid customerId, int page = 1, int pageSize = 10, CancellationToken cancellationToken = default);
    Task<Order?> GetByIdAsync(ObjectId id, CancellationToken cancellationToken = default);
    void Create(Order order);
    void Update(Order order);
    void Cancel(Order order);
}