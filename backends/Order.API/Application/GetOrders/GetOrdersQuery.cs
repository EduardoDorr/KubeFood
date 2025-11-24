using KubeFood.Core.Models.Pagination;

namespace KubeFood.Order.API.Application.GetOrders;

public sealed record GetOrdersQuery(
    PaginationParameters Pagination);