using Catalog.API.Domain;

namespace Catalog.API.Application.UpdateProduct;

public sealed record UpdateProductInputModel(
    string Name,
    string? Description,
    ProductCategory Category,
    string? ImageUrl,
    decimal Value,
    decimal Weight);

public static class UpdateProductInputModelExtensions
{
    public static UpdateProductCommand ToCommand(this UpdateProductInputModel model, string uuid)
        => new(
            uuid,
            model.Name,
            model.Description,
            model.Category,
            model.ImageUrl,
            model.Value,
            model.Weight);
}