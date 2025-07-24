using Catalog.API.Domain;

namespace Catalog.API.Application.UpdateProduct;

public sealed record UpdateProductInputModel(
    string Name,
    string? Description,
    ProductCategory Category,
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
            model.Value,
            model.Weight);
}