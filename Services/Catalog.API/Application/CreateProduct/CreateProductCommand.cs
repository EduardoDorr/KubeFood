using Catalog.API.Domain;

namespace Catalog.API.Application.CreateProduct;

public sealed record CreateProductCommand(
    string Name,
    string? Description,
    ProductCategory Category,
    string? ImageUrl,
    decimal Value,
    decimal Weight);

public static class CreateProductCommandExtensions
{
    public static Product ToProduct(this CreateProductCommand command)
        => new(
            command.Name,
            command.Description,
            command.Category,
            command.ImageUrl,
            command.Value,
            command.Weight);
}