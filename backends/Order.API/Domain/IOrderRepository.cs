using KubeFood.Core.Models.Pagination;

namespace KubeFood.Order.API.Domain;

public interface IOrderRepository
{
    Task<PaginationResult<Order>> GetAllAsync(int page = 1, int pageSize = 10, CancellationToken cancellationToken = default);
    Task<PaginationResult<Order>> GetByCustomerAsync(int customerId, int page = 1, int pageSize = 10, CancellationToken cancellationToken = default);
    Task<Order?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<Order?> GetByUniqueIdAsync(Guid uniqueId, CancellationToken cancellationToken = default);
    void Create(Order order);
    void Update(Order order);
}