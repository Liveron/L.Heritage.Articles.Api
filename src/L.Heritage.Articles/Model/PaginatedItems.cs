namespace L.Heritage.Articles.Model;

public class PaginatedItems<TEntity>(
    int pageNumber, long totalCount, bool hasPrevious, bool hasNext, IEnumerable<TEntity> data)
{
    public int PageNumber { get; init; } = pageNumber;
    public long TotalCount { get; init; } = totalCount;
    public bool HasPrevious { get; init; } = hasPrevious;
    public bool HasNext { get; init; } = hasNext;
    public IEnumerable<TEntity> Data { get; init; } = data;
}
