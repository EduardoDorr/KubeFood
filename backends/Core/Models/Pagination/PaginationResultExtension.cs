using Microsoft.EntityFrameworkCore;

namespace KubeFood.Core.Models.Pagination;

public static class PaginationResultExtension
{
    public static async Task<PaginationResult<T>> GetPagedAsync<T>(
        this IQueryable<T> query,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default) where T : class
    {
        var totalCount = await query.CountAsync(cancellationToken);

        var result =
            new PaginationResult<T>(page, pageSize, totalCount);

        var pageCount = (double)result.TotalCount / pageSize;
        var totalPages = (int)Math.Ceiling(pageCount);

        result.SetTotalPages(totalPages);

        var skip = (page - 1) * pageSize;
        var data = await query.Skip(skip).Take(pageSize).ToListAsync(cancellationToken);

        result.SetData(data);

        return result;
    }

    public static PaginationResult<T> Map<P, T>(this PaginationResult<P> pagination, IEnumerable<T> data)
        => new
            (
                pagination.Page,
                pagination.PageSize,
                pagination.TotalCount,
                pagination.TotalPages,
                data
            );
}