using KubeFood.Core.Events;
using KubeFood.Inventory.API.Application.UpdateItem;
using KubeFood.Inventory.API.Domain;

namespace KubeFood.Inventory.API.Application.ItemUpdated;

public sealed class ItemUpdatedEventHandler : EventHandlerBase<ItemUpdatedEvent>
{
    private readonly IInventoryRepository _inventoryRepository;
    private readonly IUpdateItemCommandHandler _updateItemHandler;

    public ItemUpdatedEventHandler(
        ILogger<ItemUpdatedEventHandler> logger,
        IInventoryRepository inventoryRepository,
        IUpdateItemCommandHandler updateItemHandler)
        : base(logger)
    {
        _updateItemHandler = updateItemHandler;
        _inventoryRepository = inventoryRepository;
    }

    protected override async Task ExecuteAsync(ItemUpdatedEvent @event, CancellationToken cancellationToken = default)
    {
        var item = await _inventoryRepository
            .GetByProductIdAsync(
                @event.Id,
                cancellationToken);

        if (item is null)
        {
            _logger.LogInformation("Item does not exist in Inventory");
            return;
        }

        var result = await _updateItemHandler
            .HandleAsync(new(item.Id, @event.Name, @event.Category, @event.IsActive), cancellationToken);

        var message = result.Success
            ? "succeeded"
            : "failed";

        _logger.LogInformation("Product creation {Message}", message);
    }
}