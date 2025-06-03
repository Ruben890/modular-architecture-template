using Microsoft.AspNetCore.Http;
using Shared.DTO.Request.Dtos;
using Shared.DTO.Response;

namespace Module.Auth.Domain.Interfaces
{
    public interface IAuthService
    {
        Task<ApiResponse> Login(RequestLogin request, HttpContext context);
        ApiResponse Logout(HttpContext context);
        Task<ApiResponse> RefreshToken(HttpContext context);
    }
}
