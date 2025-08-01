using KubeFood.Core.Models.Pagination;
using KubeFood.Order.API.Domain;

using Microsoft.EntityFrameworkCore;

namespace KubeFood.Order.API.Infrastructure.Persistence.Repositories;

public class OrderRepository : IOrderRepository
{
    private readonly OrderDbContext _context;

    public OrderRepository(OrderDbContext context)
    {
        _context = context;
    }

    public async Task<PaginationResult<Domain.Order>> GetAllAsync(int page = 1, int pageSize = 10, CancellationToken cancellationToken = default)
    {
        var products = _context.Orders.AsNoTracking();

        return await products
            .GetPagedAsync(page, pageSize, cancellationToken);
    }

    public async Task<PaginationResult<Domain.Order>> GetByCustomerAsync(int customerId, int page = 1, int pageSize = 10, CancellationToken cancellationToken = default)
    {
        var orders = _context.Orders
            .Where(p => p.CustomerId == customerId)
            .AsNoTracking();

        return await orders
            .GetPagedAsync(page, pageSize, cancellationToken);
    }

    public async Task<Domain.Order?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Orders
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    public void Create(Domain.Order order)
    {
        _context.Orders.Add(order);
    }

    public void Update(Domain.Order order)
    {
        _context.Orders.Update(order);
    }
}