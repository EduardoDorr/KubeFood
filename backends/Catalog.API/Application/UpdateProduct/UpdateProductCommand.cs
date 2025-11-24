using KubeFood.Catalog.API.Domain;

namespace KubeFood.Catalog.API.Application.UpdateProduct;

public sealed record UpdateProductCommand(
    string Uiid,
    string Name,
    string? Description,
    ProductCategory Category,
    decimal Value,
    decimal Weight);