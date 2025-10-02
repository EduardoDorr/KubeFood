using KubeFood.Core.Events;
using KubeFood.Core.MessageBus;
using KubeFood.Core.Persistence.UnitOfWork;
using KubeFood.Core.Results.Base;
using KubeFood.Inventory.API.Domain;
using KubeFood.Inventory.API.Domain.Events;

namespace KubeFood.Inventory.API.Application.StockReservationRequested;

public record StockReservationContext(string ProductId, int Quantity, Guid OrderId);

public sealed class StockReservationRequestedEventHandler : EventHandlerBase<StockReservationRequestedEvent>
{
    private readonly IInventoryRepository _inventoryRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMessageBusProducerService _messageBusProducerService;

    public StockReservationRequestedEventHandler(
        ILogger<StockReservationRequestedEventHandler> logger,
        IInventoryRepository inventoryRepository,
        IUnitOfWork unitOfWork,
        IMessageBusProducerService messageBusProducerService)
        : base(logger)
    {
        _inventoryRepository = inventoryRepository;
        _unitOfWork = unitOfWork;
        _messageBusProducerService = messageBusProducerService;
    }

    protected override async Task ExecuteAsync(StockReservationRequestedEvent @event, CancellationToken cancellationToken = default)
    {
        var tasks = @event.Items
            .Select(item => ValidateItemAsync(item, @event.Id, cancellationToken))
            .ToList();

        var results = await Task.WhenAll(tasks);

        var allSucceeded = results.All(result => result.Success);

        if (allSucceeded)
        {
            var saveResult = await _unitOfWork.SaveChangesAsync(cancellationToken);

            if (saveResult < 1)
            {
                _logger.LogError("Something went wrong!");

                await _messageBusProducerService.PublishAsync(
                    nameof(StockReservedEvent),
                    new StockReservedEvent(@event.Id, false),
                    cancellationToken);

                return;
            }

            await _messageBusProducerService.PublishAsync(
                nameof(StockReservedEvent),
                new StockReservedEvent(@event.Id, true),
                cancellationToken);
        }
        else
        {
            var failedItemIds = results
                .Where(i => !i.Success)
                .Select(i => i.Context.ProductId)
                .ToList();

            await _messageBusProducerService.PublishAsync(
                nameof(StockReservedEvent),
                new StockReservedEvent(@event.Id, false, failedItemIds),
                cancellationToken);
        }
    }

    private async Task<ValidationResult<StockReservationContext>> ValidateItemAsync(StockReservationRequestedItemEvent orderItem, Guid orderId, CancellationToken cancellationToken)
    {
        var context =
            new StockReservationContext(
                orderItem.Id,
                orderItem.Quantity,
                orderId);

        var item = await _inventoryRepository
            .GetByProductIdAsync(orderItem.Id, cancellationToken);

        if (item is null)
            return ValidationResult.Fail(context, InventoryItemError.NotFound);

        var validation = item.ReserveStock(orderItem.Quantity, orderId);

        return validation.Success
            ? ValidationResult.Ok(context)
            : ValidationResult.Fail(context, validation.Errors);
    }
}