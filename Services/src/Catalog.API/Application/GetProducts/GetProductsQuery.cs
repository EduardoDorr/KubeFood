using KubeFood.Core.Models.Pagination;

namespace KubeFood.Catalog.API.Application.GetProducts;

public sealed record GetProductsQuery(
    PaginationParameters Pagination);