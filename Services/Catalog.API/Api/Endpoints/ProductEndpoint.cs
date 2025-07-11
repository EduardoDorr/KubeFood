using Catalog.API.Domain;

using Core.Helpers;
using Core.Models.Pagination;
using Core.Persistence.UnitOfWork;

namespace Catalog.API.Api.Endpoints;

public static class ProductEndpoint
{
    public static WebApplication MapProductEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/v1/products")
            .WithTags("Products");

        group.MapGet("/", async (
            IProductRepository repository,
            int page,
            int pageSize,
            CancellationToken cancellationToken) =>
        {
            var products = await repository.GetAllAsync(page, pageSize, cancellationToken);
            return Results.Ok(products);
        })
        .Produces<PaginationResult<Product>>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status500InternalServerError);

        group.MapGet("/{uuid}", async (
            IProductRepository repository,
            string uuid,
            CancellationToken cancellationToken) =>
        {
            var product = await repository.GetByIdAsync(uuid, cancellationToken);
            return product is not null ? Results.Ok(product) : Results.NotFound();
        })
        .Produces<Product>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound)
        .Produces(StatusCodes.Status500InternalServerError);

        group.MapPost("/", async (
            IProductRepository repository,
            IUnitOfWork unitOfWork,
            Product product,
            CancellationToken cancellationToken) =>
        {
            if (product is null)
                return Results.BadRequest("Product cannot be null.");

            repository.Create(product);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            product.SetUuid(product.Id);

            repository.Update(product);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            return Results.Created($"/api/v1/products/{product.Id}", product);
        });

        group.MapPatch("/{uiid}", async (
            IProductRepository repository,
            IUnitOfWork unitOfWork,
            string uuid,
            decimal newPrice,
            CancellationToken cancellationToken) =>
        {
            var product = await repository.GetByIdAsync(uuid, cancellationToken);

            if (product is null)
                return Results.NotFound("Product cannot be null.");

            product.ChangePrice(newPrice);

            repository.Update(product);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            return Results.NoContent();
        });

        group.MapDelete("/{uuid}", async (
            IProductRepository repository,
            IUnitOfWork unitOfWork,
            string uuid,
            CancellationToken cancellationToken) =>
        {
            var product = await repository.GetByIdAsync(uuid, cancellationToken);

            if (product is null)
                return Results.NotFound("Product not found.");

            product.Deactivate();

            repository.Update(product);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            return Results.NoContent();
        });

        return app;
    }
}