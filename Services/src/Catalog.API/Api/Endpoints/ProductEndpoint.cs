using Catalog.API.Application.CreateProduct;
using Catalog.API.Application.DeleteProduct;
using Catalog.API.Application.GetProduct;
using Catalog.API.Application.GetProducts;
using Catalog.API.Application.GetProductsByIds;
using Catalog.API.Application.Models;
using Catalog.API.Application.UpdateProduct;
using Catalog.API.Application.UpdateProductImage;

using Core.Models.Pagination;
using Core.Results;

using Microsoft.AspNetCore.Mvc;

namespace Catalog.API.Api.Endpoints;

public static class ProductEndpoint
{
    private const string ROUTE = "/api/v1/products";

    public static WebApplication MapProductEndpoints(this WebApplication app)
    {
        var group = app.MapGroup(ROUTE)
            .WithTags("Products");

        group.MapGet("/", async (
            [AsParameters] PaginationParameters pagination,
            [FromServices] IGetProductsQueryHandler service,
            CancellationToken cancellationToken) =>
        {
            var result = await service.HandleAsync(new(pagination), cancellationToken);

            return result.Match(
                onSuccess: success => Results.Ok(result.Value),
                onFailure: error => error.ToProblemDetails());
        })
        .Produces<PaginationResult<ProductViewModel>>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status500InternalServerError);

        group.MapPost("/uiids", async (
            [FromBody] GetProductsByUuidsQuery query,
            [FromServices] IGetProductsByUuidsQueryHandler service,
            CancellationToken cancellationToken) =>
        {
            var result = await service.HandleAsync(query, cancellationToken);

            return result.Match(
                onSuccess: success => Results.Ok(result.Value),
                onFailure: error => error.ToProblemDetails());
        })
        .Produces<PaginationResult<ProductViewModel>>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status500InternalServerError);

        group.MapGet("/{uuid}", async (
            string uuid,
            [FromServices] IGetProductQueryHandler service,
            CancellationToken cancellationToken) =>
        {
            var result = await service.HandleAsync(new(uuid), cancellationToken);

            return result.Match(
                onSuccess: success => Results.Ok(result.Value),
                onFailure: error => error.ToProblemDetails());
        })
        .Produces<ProductViewModel>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound)
        .Produces(StatusCodes.Status500InternalServerError);

        group.MapPost("/", async (
            CreateProductCommand command,
            [FromServices] ICreateProductCommandHandler service,
            CancellationToken cancellationToken) =>
        {
            if (command is null)
                return Results.BadRequest("Product cannot be null.");

            var result = await service.HandleAsync(command, cancellationToken);

            return result.Match(
                onSuccess: success => Results.Created($"{ROUTE}/{success}", command),
                onFailure: error => error.ToProblemDetails());
        })
        .Produces<string>(StatusCodes.Status201Created)
        .Produces(StatusCodes.Status500InternalServerError);

        group.MapPut("/{uuid}", async (
            string uuid,
            [FromBody] UpdateProductInputModel model,
            [FromServices] IUpdateProductCommandHandler service,
            CancellationToken cancellationToken) =>
        {
            if (model is null)
                return Results.BadRequest("Product cannot be null.");

            var result = await service.HandleAsync(model.ToCommand(uuid), cancellationToken);

            return result.Match(
                onSuccess: () => Results.NoContent(),
                onFailure: error => error.ToProblemDetails());
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
            var result = await service.HandleAsync(new(uuid, model.ImageUrl), cancellationToken);

            return result.Match(
                onSuccess: () => Results.NoContent(),
                onFailure: error => error.ToProblemDetails());
        })
        .Produces(StatusCodes.Status204NoContent)
        .Produces(StatusCodes.Status404NotFound)
        .Produces(StatusCodes.Status500InternalServerError);

        group.MapDelete("/{uuid}", async (
            string uuid,
            [FromServices] IDeleteProductCommandHandler service,
            CancellationToken cancellationToken) =>
        {
            var result = await service.HandleAsync(new(uuid), cancellationToken);

            return result.Match(
                onSuccess: () => Results.NoContent(),
                onFailure: error => error.ToProblemDetails());
        })
        .Produces(StatusCodes.Status204NoContent)
        .Produces(StatusCodes.Status404NotFound)
        .Produces(StatusCodes.Status500InternalServerError);

        return app;
    }
}