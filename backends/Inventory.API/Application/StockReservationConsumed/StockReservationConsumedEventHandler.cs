using KubeFood.Core.Events;
using KubeFood.Core.Persistence.UnitOfWork;
using KubeFood.Core.Results.Base;
using KubeFood.Inventory.API.Domain;

namespace KubeFood.Inventory.API.Application.StockReservationConsumed;

internal sealed record StockReservationContext(string ProductId, int Quantity, Guid OrderId);

public sealed class StockReservationConsumedEventHandler : EventHandlerBase<StockReservationConsumedEvent>
{
    private readonly IInventoryRepository _inventoryRepository;
    private readonly IUnitOfWork _unitOfWork;

    public StockReservationConsumedEventHandler(
        ILogger<StockReservationConsumedEventHandler> logger,
        IInventoryRepository inventoryRepository,
        IUnitOfWork unitOfWork)
        : base(logger)
    {
        _inventoryRepository = inventoryRepository;
        _unitOfWork = unitOfWork;
    }

    protected override async Task ExecuteAsync(StockReservationConsumedEvent @event, CancellationToken cancellationToken = default)
    {
        var tasks = @event.Items
            .Select(item => ConfirmConsumptionItemAsync(item, @event.Id, cancellationToken))
            .ToList();

        var results = await Task.WhenAll(tasks);

        var allSucceeded = results.All(result => result.Success);

        if (allSucceeded)
        {
            var saveResult = await _unitOfWork.SaveChangesAsync(cancellationToken);

            if (saveResult < 1)
            {
                _logger.LogError("Something went wrong!");
                _logger.LogError("Reserved stock consumption not confirmed for Order {OrderId}", @event.Id);

                return;
            }

            _logger.LogInformation("Reserved stock consumption confirmed for Order {OrderId}", @event.Id);
        }
        else
        {
            var failedItemIds = results
                .Where(i => !i.Success)
                .Select(i => i.Context.ProductId)
                .ToList();

            _logger.LogError("Reserved stock consumption failed for Order {OrderId} for items: {FailedItems}", @event.Id, failedItemIds);
        }
    }

    private async Task<ValidationResult<StockReservationContext>> ConfirmConsumptionItemAsync(
        StockReservationConsumedItemEvent orderItem, Guid orderId, CancellationToken cancellationToken)
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