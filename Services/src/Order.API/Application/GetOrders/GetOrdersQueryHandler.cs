using KubeFood.Core.Interfaces;
using KubeFood.Core.Models.Pagination;
using KubeFood.Core.Results;
using KubeFood.Order.API.Application.Models;
using KubeFood.Order.API.Domain;

namespace KubeFood.Order.API.Application.GetOrders;

public interface IGetOrdersQueryHandler
    : IRequestHandler<GetOrdersQuery, Result<PaginationResult<OrderViewModel>>>
{ }

public class GetOrdersQueryHandler : IGetOrdersQueryHandler
{
    private readonly IOrderRepository _orderRepository;

    public GetOrdersQueryHandler(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }

    public async Task<Result<PaginationResult<OrderViewModel>>> HandleAsync(GetOrdersQuery request, CancellationToken cancellationToken = default)
    {
        var orders =
            await _orderRepository
                .GetAllAsync(
                    request.Pagination.Page,
                    request.Pagination.PageSize,
                    cancellationToken);

        var paginatedOrders =
            orders
                .Map(orders.Data.ToModel().ToList());

        return Result.Ok(paginatedOrders);
    }
}