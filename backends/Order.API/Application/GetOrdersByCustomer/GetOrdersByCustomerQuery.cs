using KubeFood.Core.Models.Pagination;

namespace KubeFood.Order.API.Application.GetOrdersByCustomer;

public sealed record GetOrdersByCustomerQuery(
    int CustomerId,
    PaginationParameters Pagination);