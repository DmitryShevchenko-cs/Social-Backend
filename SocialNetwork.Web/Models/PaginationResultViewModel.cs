namespace SocialNetwork.Web.Models
{
    public class PaginationResultViewModel<T>
    {
        public IEnumerable<T> Data { get; set; } = null!;
        public int CurrentPage { get; set; }
        public int TotalItems { get; set; }
        public int TotalDbItems { get; set; }
    }
}
