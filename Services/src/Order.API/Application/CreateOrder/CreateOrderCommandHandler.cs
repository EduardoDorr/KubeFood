using KubeFood.Core.Interfaces;
using KubeFood.Core.Persistence.UnitOfWork;
using KubeFood.Core.Results;
using KubeFood.Order.API.Domain;

namespace KubeFood.Order.API.Application.CreateOrder;

public interface ICreateOrderCommandHandler
    : IRequestHandler<CreateOrderCommand, Result<int>>
{ }

public class CreateOrderCommandHandler : ICreateOrderCommandHandler
{
    private readonly IOrderRepository _orderRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateOrderCommandHandler(IOrderRepository productRepository, IUnitOfWork unitOfWork)
    {
        _orderRepository = productRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<int>> HandleAsync(CreateOrderCommand request, CancellationToken cancellationToken = default)
    {
        var order = request.ToOrder();

        _orderRepository.Create(order);
        var createResult = await _unitOfWork.SaveChangesAsync(cancellationToken);

        if (createResult < 1)
            return Result.Fail<int>(OrderError.CannotCreate);

        return Result.Ok(order.Id);
    }
}