namespace redil_backend.Dtos
{
    public class PaginatedResponse<T>
    {
        public ICollection<T> Data { get; set; } = new List<T>();
        public int TotalRecords { get; set; }
        public int PageSize { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
    }
}
