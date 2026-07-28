
namespace ERP.Application.Common.Models;

using System;
using System.Collections.Generic;
using System.Linq;

public class PagedResult<T>
{
    public IReadOnlyList<T> Items { get; }
    public int Page { get; }
    public int PageSize { get; }
    public int TotalCount { get; }
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)(PageSize > 0 ? PageSize : 1));
    public bool HasPreviousPage => Page > 1;
    public bool HasNextPage => Page < TotalPages;

    public PagedResult(IReadOnlyList<T> items, int totalCount, int page, int pageSize)
    {
        Items = items ?? new List<T>();
        TotalCount = totalCount;
        Page = page <= 0 ? 1 : page;
        PageSize = pageSize <= 0 ? 20 : pageSize;
    }

    public PagedResult<TTarget> Map<TTarget>(Func<T, TTarget> selector)
    {
        var mappedItems = Items.Select(selector).ToList();
        return new PagedResult<TTarget>(mappedItems, TotalCount, Page, PageSize);
    }
}
