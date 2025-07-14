using Catalog.API.Domain;

namespace Catalog.API.Application.UpdateProduct;

public sealed record UpdateProductCommand(
    string Uiid,
    string Name,
    string? Description,
    ProductCategory Category,
    string? ImageUrl,
    decimal Value,
    decimal Weight);