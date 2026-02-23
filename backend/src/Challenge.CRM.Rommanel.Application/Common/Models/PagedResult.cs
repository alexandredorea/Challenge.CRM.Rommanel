using Microsoft.EntityFrameworkCore;

namespace Challenge.CRM.Rommanel.Application.Common.Models;

public sealed record PagedResult<T>(
    IReadOnlyList<T> Items,
    int TotalCount,
    int Page,
    int PageSize)
{
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
    public bool HasNextPage => Page < TotalPages;
    public bool HasPreviousPage => Page > 1;
    public bool IsFirstPage => Page == 1;
    public bool IsLastPage => Page >= TotalPages;
}

public static class QueryableExtensions
{
    private const int MaxPageSize = 100;
    private const int DefaultPageSize = 10;

    public static async Task<PagedResult<TDestination>> ToPaginatedListAsync<TDestination>(
        this IQueryable<TDestination> query,
        int page = 1,
        int pageSize = DefaultPageSize,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(query);

        if (page < 1)
            page = 1;

        if (pageSize < 1)
            pageSize = DefaultPageSize;

        if (pageSize > MaxPageSize)
            pageSize = MaxPageSize;

        var totalCount = await query.CountAsync(cancellationToken);

        if (totalCount == 0)
            return new PagedResult<TDestination>([], page, pageSize, 0);

        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<TDestination>(items, page, pageSize, totalCount);
    }

    public static PagedResult<TDestination> Map<TSource, TDestination>(
        this PagedResult<TSource> source,
        Func<TSource, TDestination> mapper)
    {
        return new PagedResult<TDestination>(
            source.Items?.Select(mapper).ToList()!,
            source.Page,
            source.PageSize,
            source.TotalCount);
    }
}