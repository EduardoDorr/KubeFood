using Catalog.API.Domain;

namespace Catalog.API.Application.UpdateProduct;

public sealed record UpdateProductCommand(
    string Uiid,
    string Name,
    string? Description,
    ProductCategory Category,
    decimal Value,
    decimal Weight);