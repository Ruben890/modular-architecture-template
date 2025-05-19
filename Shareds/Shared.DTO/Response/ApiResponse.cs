using Shared.DTO.Request;
using System.Net;

namespace Shared.DTO.Response
{
    public class ApiResponse
    {
        public string? Message { get; set; } = null!;
        public HttpStatusCode StatusCode { get; set; }
        public object? Details { get; set; } = null!;
        public Pagination? Pagination { get; set; } = null!;
        public void SetPagination(Pagination pagination)
        {
            Pagination = new Pagination
            {
                CurrentPage = pagination.CurrentPage,
                TotalPages = pagination.TotalPages,
                PreviousPage = pagination.PreviousPage,
                NextPage = pagination.NextPage,
                TotalCount = pagination.TotalCount,
                PageSize = pagination.PageSize
            };
        }
    }
}
