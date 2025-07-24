namespace Catalog.API.Application.GetProductsByIds;

public sealed record GetProductsByUuidsQuery(
    List<string> ProductUuids);