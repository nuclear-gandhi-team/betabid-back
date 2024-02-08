using Betabid.Application.DTOs.FilteringDto;
using Microsoft.EntityFrameworkCore;

namespace Betabid.Persistence.Extensions;

public static class PagingExtension
{
    public static async Task<PagedResult<T>> GetPaged<T>(this IQueryable<T> query, IPageable pageable)
    {
        var count = await query.CountAsync();
        if (count == 0)
        {
            return new PagedResult<T>
            {
                Items = new List<T>(),
                TotalPages = 0
            };
        }
        if (pageable.Page != null)
        {
            query = query.Skip((pageable.Page.Value - 1) * pageable.PageCount);
        }
        query = query.Take(pageable.PageCount);
        return new PagedResult<T>
        {
            Items = await query.ToListAsync(),
            TotalPages = (int)Math.Ceiling(count / (double)pageable.PageCount)
        };
    }

}