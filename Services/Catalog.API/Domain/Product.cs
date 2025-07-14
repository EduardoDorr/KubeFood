using Core.Entities;
using Core.Helpers;
using Core.Results;

namespace Catalog.API.Domain;

public sealed class Product : BaseMongoEntity
{
    public string Uuid { get; private set; }
    public string Name { get; private set; }
    public string? Description { get; private set; }
    public ProductCategory Category { get; private set; }
    public string? ImageUrl { get; private set; }
    public decimal Value { get; private set; }
    public decimal Weight { get; private set; }

    private Product() { }

    public Product(
        string name,
        string? description,
        ProductCategory category,
        string? imageUrl,
        decimal value,
        decimal weight)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Nome é obrigatório", nameof(name));

        if (value < 0)
            throw new ArgumentOutOfRangeException(nameof(value), "Valor não pode ser negativo");

        if (weight < 0)
            throw new ArgumentOutOfRangeException(nameof(weight), "Peso não pode ser negativo");

        Name = name;
        Description = description;
        Category = category;
        ImageUrl = imageUrl;
        Value = value;
        Weight = weight;
    }

    public void SetUuid(string id)
    {
        Uuid = HashIdHelper.EncodeId(id);
    }

    public Result Update(
        string name,
        string? description,
        ProductCategory category,
        string? imageUrl,
        decimal value,
        decimal weight)
    {
        if (string.IsNullOrWhiteSpace(name))
            return Result.Fail(ProductError.CannotUpdate);

        if (value < 0)
            return Result.Fail(ProductError.CannotUpdate);

        if (weight < 0)
            return Result.Fail(ProductError.CannotUpdate);

        Name = name;
        Description = description;
        Category = category;
        ImageUrl = imageUrl;
        Value = value;
        Weight = weight;

        SetUpdatedAtDate(DateTime.UtcNow);

        return Result.Ok();
    }
}