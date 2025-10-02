using KubeFood.Core.Events;
using KubeFood.Inventory.API.Application.CreateItem;

namespace KubeFood.Inventory.API.Application.ItemCreated;

public sealed class ItemCreatedEventHandler : EventHandlerBase<ItemCreatedEvent>
{
    private readonly ICreateItemCommandHandler _createItemHandler;

    public ItemCreatedEventHandler(
        ILogger<ItemCreatedEventHandler> logger,
        ICreateItemCommandHandler createItemHandler)
        : base(logger)
    {
        _createItemHandler = createItemHandler;
    }

    protected override async Task ExecuteAsync(ItemCreatedEvent @event, CancellationToken cancellationToken = default)
    {
        var result = await _createItemHandler
            .HandleAsync(new(@event.Id, @event.Name, @event.Category), cancellationToken);

        var message = result.Success
            ? "succeeded"
            : "failed";

        _logger.LogInformation("Item creation {Message}", message);
    }
}