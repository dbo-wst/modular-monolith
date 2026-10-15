namespace Template.Core.Ordering.Infrastructure;

internal static class Paging
{
    private const int DefaultPageSize = 10;
    private const int MaxPageSize = 100;
    private const int MaxPageNumber = 1000;

    public static (int Skip, int Take) Normalize(int pageNumber, int pageSize)
    {
        var page = Math.Min(MaxPageNumber, Math.Max(pageNumber, 1));
        var size = pageSize <= 0 ? DefaultPageSize : Math.Min(pageSize, MaxPageSize);
        return ((page - 1) * size, size);
    }
}
