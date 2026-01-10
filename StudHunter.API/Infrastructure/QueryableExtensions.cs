using Microsoft.EntityFrameworkCore;
using StudHunter.API.ModelsDto;

namespace StudHunter.API.Infrastructure;

public static class QueryableExtensions
{
    public static async Task<PagedResult<T>> ToPagedResultAsync<T>(
        this IQueryable<T> query,
        PaginationParams paging)
    {
        var totalCount = await query.CountAsync();

        var items = await query
            .Skip((paging.PageNumber - 1) * paging.PageSize)
            .Take(paging.PageSize)
            .ToListAsync();

        return new PagedResult<T>(items, totalCount, paging.PageNumber, paging.PageSize);
    }
}