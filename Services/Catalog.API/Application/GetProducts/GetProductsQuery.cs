using Core.Models.Pagination;

namespace Catalog.API.Application.GetProducts;

public sealed record GetProductsQuery(
    PaginationParameters Pagination);