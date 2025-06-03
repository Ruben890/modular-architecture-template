using Microsoft.AspNetCore.Http;
using Module.User.Domain.Interfaces.IRepository;
using Module.User.Domain.Interfaces.IServices;
using Shared.Core.Extensions;
using Shared.Core.Interfaces;
using Shared.Core.Utils;
using Shared.DTO.Request.QueryParameters;
using Shared.DTO.Response;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using UserDTO = Shared.DTO.Dtos.User;

namespace Module.User.Application.Services
{
    public class UserServices : IUserServices
    {
        private readonly ILoggerManager _logger;
        private readonly IUserRepository  _userRepository;


        public UserServices(ILoggerManager logger, IUserRepository userRepository)
        {
            _logger = logger;
            _userRepository = userRepository;
        }

        public async Task<ApiResponse> GetByUser(GenericParameters parameters, HttpContext context)
        {
            var response = new ApiResponse();

            if (!parameters.UserId.HasValue)
            {
                context.Request.Cookies.TryGetValue("accessToken", out var accessToken);

                if (string.IsNullOrWhiteSpace(accessToken))
                {
                    return response.CustomResponse("Access token not found in cookies.", HttpStatusCode.Unauthorized);
                }

                try
                {

                    var handler = new JwtSecurityTokenHandler();
                    var jsonToken = handler.ReadToken(accessToken) as JwtSecurityToken;

                    var userId = jsonToken?.Claims.FirstOrDefault(c => c.Type == "Id")?.Value;

                    if (string.IsNullOrWhiteSpace(userId))
                        return response.CustomResponse("The 'User' parameter is required and cannot be empty or contain only blank spaces.");

                    if (!Guid.TryParse(userId, out var parsedUserId))
                        return response.CustomResponse("The token's UserId is not a valid GUID.", HttpStatusCode.BadRequest);

                    parameters.UserId = parsedUserId;
                }
                catch (Exception ex)
                {
                    return response.CustomResponse($"Error processing token: {ex.Message}", HttpStatusCode.Unauthorized);
                }
            }

            var getByUser = await _userRepository.GetUserById(parameters.UserId.Value);

            var user = getByUser.Adapt<UserDTO>();

            if (getByUser is null)
                return response.CustomResponse("User not found.", HttpStatusCode.NotFound);

            if (!string.IsNullOrWhiteSpace(user.Dni))
                user.Dni = Encrypt.DecryptString(user.Dni);

            // Clear sensitive information
            user.RefreshToken = null;
            user.RefreshTokenExpiryTime = null;
            user.Password = null;
            user.Company = await _bus.InvokeAsync<CompanyDTO>(new GetCompanyByUserId(getByUser.Id));

            return response.CustomResponse(null!, HttpStatusCode.OK, user);

        }

    }
}
