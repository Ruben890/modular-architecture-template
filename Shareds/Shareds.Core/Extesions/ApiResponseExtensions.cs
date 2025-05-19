using System.Net;
using Shared.DTO.Request;
using Shared.DTO.Response;

namespace Shareds.Core.Extesions
{
    public static class ApiResponseExtensions
    {
        public static ApiResponse CustomResponse<T>(this T data, string message,
            HttpStatusCode statusCode = HttpStatusCode.BadRequest, Pagination? pagination = null)
        {
            var response = new ApiResponse
            {
                Details = data,
                Message = message,
                StatusCode = statusCode
            };

            if (pagination != null)
            {
                response.SetPagination(pagination);
            }

            return response;
        }
    }
}
