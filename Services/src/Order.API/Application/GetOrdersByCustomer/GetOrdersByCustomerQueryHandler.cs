using KubeFood.Core.Interfaces;
using KubeFood.Core.Models.Pagination;
using KubeFood.Core.Results.Base;
using KubeFood.Order.API.Application.Models;
using KubeFood.Order.API.Domain;

namespace KubeFood.Order.API.Application.GetOrdersByCustomer;

public interface IGetOrdersByCustomerQueryHandler
    : IRequestHandler<GetOrdersByCustomerQuery, Result<PaginationResult<OrderViewModel>>>
{ }

public class GetOrdersByCustomerQueryHandler : IGetOrdersByCustomerQueryHandler
{
    private readonly IOrderRepository _orderRepository;

    public GetOrdersByCustomerQueryHandler(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }

    public async Task<Result<PaginationResult<OrderViewModel>>> HandleAsync(GetOrdersByCustomerQuery request, CancellationToken cancellationToken = default)
    {
        var orders =
            await _orderRepository
                .GetByCustomerAsync(
                    request.CustomerId,
                    request.Pagination.Page,
                    request.Pagination.PageSize,
                    cancellationToken);

        var paginatedOrders =
            orders
                .Map(orders.Data.ToModel().ToList());

        return Result.Ok(paginatedOrders);
    }
}