using KubeFood.Core.Interfaces;
using KubeFood.Core.Persistence.UnitOfWork;
using KubeFood.Core.Results.Base;
using KubeFood.Order.API.Domain;

namespace KubeFood.Order.API.Application.CancelOrder;

public interface ICancelOrderCommandHandler
    : IRequestHandler<CancelOrderCommand, Result>
{ }

public class CancelOrderCommandHandler : ICancelOrderCommandHandler
{
    private readonly IOrderRepository _orderRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CancelOrderCommandHandler(IOrderRepository orderRepository, IUnitOfWork unitOfWork)
    {
        _orderRepository = orderRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> HandleAsync(CancelOrderCommand request, CancellationToken cancellationToken = default)
    {
        var order =
            await _orderRepository
                .GetByIdAsync(
                    request.Id,
                    cancellationToken);

        if (order is null)
            return Result.Fail(OrderError.NotFound);

        order.Deactivate();

        _orderRepository.Update(order);
        var saveResult = await _unitOfWork.SaveChangesAsync(cancellationToken);

        if (saveResult <= 0)
            return Result.Fail(OrderError.CannotDelete);

        return Result.Ok();
    }
}