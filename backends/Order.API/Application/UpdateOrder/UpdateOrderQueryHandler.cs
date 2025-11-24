using KubeFood.Core.Interfaces;
using KubeFood.Core.Persistence.UnitOfWork;
using KubeFood.Core.Results.Base;
using KubeFood.Order.API.Domain;

namespace KubeFood.Order.API.Application.UpdateOrder;

public interface IUpdateOrderCommandHandler
    : IRequestHandler<UpdateOrderCommand, Result>
{ }

public class UpdateOrderCommandHandler : IUpdateOrderCommandHandler
{
    private readonly IOrderRepository _orderRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateOrderCommandHandler(IOrderRepository orderRepository, IUnitOfWork unitOfWork)
    {
        _orderRepository = orderRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> HandleAsync(UpdateOrderCommand request, CancellationToken cancellationToken = default)
    {
        var order =
            await _orderRepository
                .GetByIdAsync(
                    request.Id,
                    cancellationToken);

        if (order is null)
            return Result.Fail(OrderError.NotFound);

        order.SetDeliveryAddress(request.DeliveryAddress);

        _orderRepository.Update(order);
        var updateResult = await _unitOfWork.SaveChangesAsync(cancellationToken);

        if (updateResult <= 0)
            return Result.Fail(OrderError.CannotUpdate);

        return Result.Ok();
    }
}