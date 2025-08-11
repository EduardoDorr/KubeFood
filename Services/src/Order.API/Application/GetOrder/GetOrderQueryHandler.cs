using KubeFood.Core.Interfaces;
using KubeFood.Core.Results.Base;
using KubeFood.Order.API.Application.Models;
using KubeFood.Order.API.Domain;

namespace KubeFood.Order.API.Application.GetOrder;

public interface IGetOrderQueryHandler
    : IRequestHandler<GetOrderQuery, Result<OrderViewModel?>>
{ }

public class GetOrderQueryHandler : IGetOrderQueryHandler
{
    private readonly IOrderRepository _orderRepository;

    public GetOrderQueryHandler(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }

    public async Task<Result<OrderViewModel?>> HandleAsync(GetOrderQuery request, CancellationToken cancellationToken = default)
    {
        var order =
            await _orderRepository
                .GetByIdAsync(
                    request.Id,
                    cancellationToken);

        if (order is null)
            return Result.Fail<OrderViewModel?>(OrderError.NotFound);

        return Result.Ok(order.ToModel());
    }
}