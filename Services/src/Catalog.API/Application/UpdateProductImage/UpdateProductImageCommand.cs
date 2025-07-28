namespace KubeFood.Catalog.API.Application.UpdateProductImage;

public sealed record UpdateProductImageCommand(
    string Uiid,
    string ImageUrl);