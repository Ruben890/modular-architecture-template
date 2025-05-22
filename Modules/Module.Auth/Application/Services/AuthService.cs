using Module.Auth.Domain.Interfaces;
using Shared.Core.Extensions;
using Shared.Core.Extesions;
using Shared.DTO.Dtos;
using Shared.DTO.Request.Dtos;
using Shared.DTO.Response;
using Shared.Messages.Queries;
using System.Net;
using System.Text.Json;
using Wolverine;

namespace Module.Auth.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IMessageBus _bus;

        public AuthService(IMessageBus bus)
        {
            _bus = bus;
        }

        public async Task<ApiResponse> Login(object jsonIn)
        {
            try
            {
                var response = new ApiResponse();
                var user = new User();
                var request = JsonSerializer.Deserialize<RequestLogin>(jsonIn.ToString()!);

                if (request == null)
                    return request.CustomResponse("Invalid request. Could not deserialize input.", HttpStatusCode.BadRequest);

                if ((string.IsNullOrWhiteSpace(request.Email) && string.IsNullOrWhiteSpace(request.UserName)) ||
                    (!string.IsNullOrWhiteSpace(request.Email) && !string.IsNullOrWhiteSpace(request.UserName)))
                {
                    return request.CustomResponse("Please provide either Email or Username, not both or neither.", HttpStatusCode.BadRequest);
                }

                if (!string.IsNullOrWhiteSpace(request.Email) && string.IsNullOrWhiteSpace(request.UserName))
                {
                    var query = new GetUserByEmailOrUserName(request.Email, null!);
                    user = await _bus.InvokeAsync<User?>(query);
                    if (user == null)
                        return request.CustomResponse("No user found with the provided email.", HttpStatusCode.NotFound);
                }

                if (string.IsNullOrWhiteSpace(request.Email) && !string.IsNullOrWhiteSpace(request.UserName))
                {
                    var query = new GetUserByEmailOrUserName(null!, request.UserName);
                    user = await _bus.InvokeAsync<User?>(query);
                    if (user == null)
                        return request.CustomResponse("No user found with the provided username.", HttpStatusCode.NotFound);
                }

     
                return request.CustomResponse("Login successful.",  HttpStatusCode.OK);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
