using Shared.DTO.Request.Dtos;
using Shared.DTO.Response;

namespace Module.Auth.Domain.Interfaces
{
    public interface IAuthService
    {
        Task<ApiResponse> Login(RequestLogin jsonIn);
    }
}
