using Catalog.API.Domain;

namespace Catalog.API.Application.Models;

public sealed record ProductViewModel(
    string Uuid,
    string Name,
    string? Description,
    ProductCategory Category,
    string? ImageUrl,
    decimal Value,
    decimal Weight);

public static class ProductViewModelExtensions
{
    public static ProductViewModel? ToModel(this Product product)
        => product is null
            ? null
            :new(
                product.Uuid,
                product.Name,
                product.Description,
                product.Category,
                product.ImageUrl,
                product.Value,
                product.Weight);

    public static IEnumerable<ProductViewModel> ToModel(this IEnumerable<Product> products)
        => products is null
            ? []
            : products.Select(p => p.ToModel()!);
}