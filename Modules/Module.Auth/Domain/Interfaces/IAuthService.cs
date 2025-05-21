using Shared.DTO.Response;

namespace Module.Auth.Domain.Interfaces
{
    public interface IAuthService
    {
        Task<ApiResponse> Login(object jsonIn);
    }
}
