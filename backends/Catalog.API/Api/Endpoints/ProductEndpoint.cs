using KubeFood.Catalog.API.Application.CreateProduct;
using KubeFood.Catalog.API.Application.DeleteProduct;
using KubeFood.Catalog.API.Application.GetByIdProduct;
using KubeFood.Catalog.API.Application.GetProducts;
using KubeFood.Catalog.API.Application.GetProductsByIds;
using KubeFood.Catalog.API.Application.Models;
using KubeFood.Catalog.API.Application.UpdateProduct;
using KubeFood.Catalog.API.Application.UpdateProductImage;
using KubeFood.Core.Filters;
using KubeFood.Core.Models.Pagination;
using KubeFood.Core.Results.Api;

using Microsoft.AspNetCore.Mvc;

namespace KubeFood.Catalog.API.Api.Endpoints;

public static class ProductEndpoint
{
    private const string ROUTE = "/api/v1/products";

    public static WebApplication MapProductEndpoints(this WebApplication app)
    {
        var group = app.MapGroup(ROUTE)
            .AddEndpointFilter<ApiResultEndpointFilter>()
            .WithTags("Products");

        group.MapGet("/", async (
            [AsParameters] PaginationParameters pagination,
            [FromServices] IGetProductsQueryHandler service,
            CancellationToken cancellationToken) =>
        {
            return await service.HandleAsync(new(pagination), cancellationToken);
        })
        .Produces<ApiResult<PaginationResult<ProductViewModel>>>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status500InternalServerError);

        group.MapPost("/uuids", async (
            [FromBody] GetProductsByUuidsQuery query,
            [FromServices] IGetProductsByUuidsQueryHandler service,
            CancellationToken cancellationToken) =>
        {
            return await service.HandleAsync(query, cancellationToken);
        })
        .Produces<PaginationResult<ProductViewModel>>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status500InternalServerError);

        group.MapGet("/{uuid}", async (
            string uuid,
            [FromServices] IGetProductQueryHandler service,
            CancellationToken cancellationToken) =>
        {
            return await service.HandleAsync(new(uuid), cancellationToken);
        })
        .Produces<ProductViewModel>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound)
        .Produces(StatusCodes.Status500InternalServerError);

        group.MapPost("/", async (
            CreateProductCommand command,
            [FromServices] ICreateProductCommandHandler service,
            CancellationToken cancellationToken) =>
        {
            return await service.HandleAsync(command, cancellationToken);
        })
        .Produces<string>(StatusCodes.Status201Created)
        .Produces(StatusCodes.Status500InternalServerError);

        group.MapPut("/{uuid}", async (
            string uuid,
            [FromBody] UpdateProductInputModel model,
            [FromServices] IUpdateProductCommandHandler service,
            CancellationToken cancellationToken) =>
        {
            return await service.HandleAsync(model.ToCommand(uuid), cancellationToken);
        })
        .Produces(StatusCodes.Status204NoContent)
        .Produces(StatusCodes.Status404NotFound)
        .Produces(StatusCodes.Status500InternalServerError);

        group.MapPatch("/{uuid}/image", async (
            string uuid,
            [FromBody] UpdateProductImageInputModel model,
            [FromServices] IUpdateProductImageCommandHandler service,
            CancellationToken cancellationToken) =>
        {
            return await service.HandleAsync(new(uuid, model.ImageUrl), cancellationToken);
        })
        .Produces(StatusCodes.Status204NoContent)
        .Produces(StatusCodes.Status404NotFound)
        .Produces(StatusCodes.Status500InternalServerError);

        group.MapDelete("/{uuid}", async (
            string uuid,
            [FromServices] IDeleteProductCommandHandler service,
            CancellationToken cancellationToken) =>
        {
            return await service.HandleAsync(new(uuid), cancellationToken);
        })
        .Produces(StatusCodes.Status204NoContent)
        .Produces(StatusCodes.Status404NotFound)
        .Produces(StatusCodes.Status500InternalServerError);

        return app;
    }
}