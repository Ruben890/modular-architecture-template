using Microsoft.AspNetCore.Http;
using Shared.DTO.Request.QueryParameters;
using Shared.DTO.Response;

namespace Module.User.Domain.Interfaces.IServices
{
    public interface IUserServices
    {
        Task<ApiResponse> GetByUser(GenericParameters parameters, HttpContext context);
    }
}
