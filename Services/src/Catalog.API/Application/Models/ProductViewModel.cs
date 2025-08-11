using KubeFood.Catalog.API.Domain;
using KubeFood.Core.Helpers;

namespace KubeFood.Catalog.API.Application.Models;

public sealed record ProductViewModel(
    string Id,
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
                product.Id.EncodeId(),
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