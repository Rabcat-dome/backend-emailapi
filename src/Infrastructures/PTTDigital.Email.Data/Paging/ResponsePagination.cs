namespace PTTDigital.Email.Data.Paging
{
    public class ResponsePagination<T>
    {
        public int TotalRecord { get; set; }

        public IEnumerable<T>? Data { get; set; }
    }
}
