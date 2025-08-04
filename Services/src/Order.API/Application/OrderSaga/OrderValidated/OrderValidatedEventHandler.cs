using KubeFood.Core.MessageBus;
using KubeFood.Order.API.Domain;

namespace KubeFood.Order.API.Application.OrderSaga.OrderValidated;

public class OrderValidatedEventHandler : MessageBusConsumerServiceHandlerBase<OrderValidatedEvent>
{
    private readonly IOrderRepository _orderRepository;

    public OrderValidatedEventHandler(
        ILogger<OrderValidatedEventHandler> logger,
        IOrderRepository orderRepository)
        : base(logger)
    {
        _orderRepository = orderRepository;
    }

    protected async override Task<MessageBusConsumerResult> ExecuteAsync(OrderValidatedEvent message, CancellationToken cancellationToken = default)
    {
        await Task.Delay(1000, cancellationToken);

        return MessageBusConsumerResult.Ack;
    }
}