namespace KubeFood.Core.Models.Pagination;

public sealed record PaginationParameters(int Page = 1, int PageSize = 10);