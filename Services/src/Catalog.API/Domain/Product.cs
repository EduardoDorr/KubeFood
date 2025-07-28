using KubeFood.Core.Entities;
using KubeFood.Core.Helpers;
using KubeFood.Core.Results;

using MongoDB.Bson;

namespace KubeFood.Catalog.API.Domain;

public sealed class Product : BaseMongoEntity
{
    public string? Uuid { get; private set; }
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
        Value = value;
        Weight = weight;
    }

    public void SetUuid(ObjectId id)
    {
        Uuid = id.EncodeId();
    }

    public Result Update(
        string name,
        string? description,
        ProductCategory category,
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
        Value = value;
        Weight = weight;

        SetUpdatedAtDate(DateTime.UtcNow);

        return Result.Ok();
    }

    public Result SetImage(string imageUrl)
    {
        if (string.IsNullOrWhiteSpace(imageUrl))
            return Result.Fail(ProductError.WithoutImage);

        ImageUrl = imageUrl;

        SetUpdatedAtDate(DateTime.UtcNow);

        return Result.Ok();
    }
}