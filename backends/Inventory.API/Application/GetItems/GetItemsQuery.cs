using KubeFood.Core.Models.Pagination;

namespace KubeFood.Inventory.API.Application.GetItems;

public sealed record GetItemsQuery(
    PaginationParameters Pagination,
    string? ProductName = null);