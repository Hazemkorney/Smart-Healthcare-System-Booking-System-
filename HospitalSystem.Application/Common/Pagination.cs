namespace HospitalSystem.Application.Common;

public static class Pagination
{
    public const int DefaultPage = 1;
    public const int DefaultPageSize = 10;
    public const int MaxPageSize = 100;

    public static (int Page, int PageSize) Normalize(int page, int pageSize)
    {
        page = page < 1 ? DefaultPage : page;
        pageSize = pageSize < 1 ? DefaultPageSize : Math.Min(pageSize, MaxPageSize);
        return (page, pageSize);
    }

    public static PagedResponse<T> Create<T>(IEnumerable<T> source, int page, int pageSize)
    {
        var (p, ps) = Normalize(page, pageSize);
        var list = source as IList<T> ?? source.ToList();
        var totalCount = list.Count;
        var items = list.Skip((p - 1) * ps).Take(ps).ToList();

        return new PagedResponse<T>
        {
            Data = items,
            TotalCount = totalCount,
            Page = p,
            PageSize = ps,
            TotalPages = totalCount == 0 ? 0 : (int)Math.Ceiling(totalCount / (double)ps)
        };
    }
}
