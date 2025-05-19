namespace Shared.DTO.Request
{
    public class PagedList<T> : List<T>
    {
        public Pagination Pagination { get; set; }

        public PagedList(List<T> items, int count, int pageNumber, int pageSize)
        {
            Pagination = new Pagination
            {
                TotalCount = count,
                PageSize = pageSize,
                CurrentPage = pageNumber,
                TotalPages = (int)Math.Ceiling(count / (double)pageSize),
                PreviousPage = pageNumber > 1 ? pageNumber - 1 : (int?)null,
                NextPage = pageNumber < (int)Math.Ceiling(count / (double)pageSize) ? pageNumber + 1 : (int?)null
            };
            AddRange(items);
        }
    }
}
