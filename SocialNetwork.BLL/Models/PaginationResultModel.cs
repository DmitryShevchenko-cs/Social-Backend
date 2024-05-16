public class PaginationResultModel<T>
{
    public IEnumerable<T> Data { get; set; } = null!;
    public int CurrentPage { get; set; }
    public int PageSize { get; set; }
    public int TotalItems { get; set; }

    public int TotalDbItems { get; set; }
}
