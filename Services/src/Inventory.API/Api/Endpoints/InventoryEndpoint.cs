using KubeFood.Core.Filters;
using KubeFood.Core.Models.Pagination;
using KubeFood.Core.Results.Api;
using KubeFood.Inventory.API.Application.AddStockToItem;
using KubeFood.Inventory.API.Application.CreateItem;
using KubeFood.Inventory.API.Application.DeleteItem;
using KubeFood.Inventory.API.Application.GetItemById;
using KubeFood.Inventory.API.Application.GetItemByProductId;
using KubeFood.Inventory.API.Application.GetItems;
using KubeFood.Inventory.API.Application.Models;
using KubeFood.Inventory.API.Application.RemoveStockFromItem;

using Microsoft.AspNetCore.Mvc;

namespace KubeFood.Inventory.API.Api.Endpoints;

public static class InventoryEndpoint
{
    private const string ROUTE = "/api/v1/inventory";

    public static WebApplication MapInventoryEndpoints(this WebApplication app)
    {
        var group = app.MapGroup(ROUTE)
            .AddEndpointFilter<ApiResultEndpointFilter>()
            .WithTags("Inventory");

        group.MapGet("/", async (
            [AsParameters] PaginationParameters pagination,
            [FromServices] IGetItemsQueryHandler service,
            CancellationToken cancellationToken) =>
        {
            return await service.HandleAsync(new(pagination), cancellationToken);
        })
        .Produces<ApiResult<PaginationResult<ItemViewModel>>>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status500InternalServerError);

        group.MapGet("/{id}", async (
            int id,
            [FromServices] IGetItemByIdQueryHandler service,
            CancellationToken cancellationToken) =>
        {
            return await service.HandleAsync(new(id), cancellationToken);
        })
        .Produces<ItemViewModel>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound)
        .Produces(StatusCodes.Status500InternalServerError);

        group.MapGet("/productId/{productId}", async (
            string productId,
            [FromServices] IGetItemByProductIdQueryHandler service,
            CancellationToken cancellationToken) =>
        {
            return await service.HandleAsync(new(productId), cancellationToken);
        })
        .Produces<ItemViewModel>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound)
        .Produces(StatusCodes.Status500InternalServerError);

        group.MapPost("/", async (
            CreateItemCommand command,
            [FromServices] ICreateItemCommandHandler service,
            CancellationToken cancellationToken) =>
        {
            return await service.HandleAsync(command, cancellationToken);
        })
        .Produces<string>(StatusCodes.Status201Created)
        .Produces(StatusCodes.Status500InternalServerError);

        group.MapPut("/{id}/add-stock", async (
            int id,
            [FromBody] AddStockToItemInputModel model,
            [FromServices] IAddStockToItemCommandHandler service,
            CancellationToken cancellationToken) =>
        {
            return await service.HandleAsync(model.ToCommand(id), cancellationToken);
        })
        .Produces(StatusCodes.Status204NoContent)
        .Produces(StatusCodes.Status404NotFound)
        .Produces(StatusCodes.Status500InternalServerError);

        group.MapPut("/{id}/remove-stock", async (
            int id,
            [FromBody] RemoveStockFromItemInputModel model,
            [FromServices] IRemoveStockFromItemCommandHandler service,
            CancellationToken cancellationToken) =>
        {
            return await service.HandleAsync(model.ToCommand(id), cancellationToken);
        })
        .Produces(StatusCodes.Status204NoContent)
        .Produces(StatusCodes.Status404NotFound)
        .Produces(StatusCodes.Status500InternalServerError);

        group.MapDelete("/{id}", async (
            int id,
            [FromServices] IDeleteItemCommandHandler service,
            CancellationToken cancellationToken) =>
        {
            return await service.HandleAsync(new(id), cancellationToken);
        })
        .Produces(StatusCodes.Status204NoContent)
        .Produces(StatusCodes.Status404NotFound)
        .Produces(StatusCodes.Status500InternalServerError);

        return app;
    }
}