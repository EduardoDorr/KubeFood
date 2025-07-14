using System.Collections.Immutable;

namespace Core.Models.Pagination;

public class PaginationResult<T>
{
    public int Page { get; }
    public int PageSize { get; }
    public int TotalCount { get; }
    public int TotalPages { get; private set; }
    public bool HasNextPage => Page < TotalPages;
    public bool HasPreviousPage => Page > 1;
    public IReadOnlyList<T> Data { get; private set; } = [];

    public PaginationResult() { }

    public PaginationResult(int page, int pageSize, int totalCount)
    {
        Page = page;
        PageSize = pageSize;
        TotalCount = totalCount;
    }

    public PaginationResult(int page, int pageSize, int totalCount, int totalPages, IEnumerable<T> data)
    {
        Page = page;
        PageSize = pageSize;
        TotalCount = totalCount;
        TotalPages = totalPages;
        Data = data.ToImmutableList();
    }    

    public void SetTotalPages(int total) => TotalPages = total;
    public void SetData(IEnumerable<T> values) => Data = values.ToImmutableList();
}