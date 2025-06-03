using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Module.Auth.Domain.Interfaces;
using Shared.Core.Extensions;
using Shared.Core.Utils;
using Shared.DTO.Dtos;
using Shared.DTO.Dtos.Shared.DTO.Enums;
using Shared.DTO.Request.Dtos;
using Shared.DTO.Response;
using Shared.Messages.Commads;
using Shared.Messages.Queries;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using Wolverine;
using UserDto = Shared.DTO.Dtos.User;
namespace Module.Auth.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IMessageBus _bus;
        private readonly IHostEnvironment _environment;
        public AuthService(IMessageBus bus, IHostEnvironment environment)
        {
            _bus = bus;
            _environment = environment;
        }

        public async Task<ApiResponse> Login(RequestLogin request, HttpContext context)
        {
            try
            {
                var user = new UserDto();

                if (request == null)
                    return request.CustomResponse("Invalid request. Could not deserialize input.", HttpStatusCode.BadRequest);

                var isEmail = new EmailAddressAttribute().IsValid(request.EmailOrUserName);

                if (string.IsNullOrWhiteSpace(request.EmailOrUserName))
                {
                    return request.CustomResponse("Please provide either Email or Username, not both or neither.", HttpStatusCode.BadRequest);
                }

                if (!string.IsNullOrWhiteSpace(request.EmailOrUserName) && isEmail)
                {
                    var query = new GetUserByEmailOrUserName(request.EmailOrUserName, null!);
                    user = await _bus.InvokeAsync<UserDto?>(query);
                    if (user == null)
                        return request.CustomResponse("No user found with the provided email.", HttpStatusCode.NotFound);
                }

                if (!string.IsNullOrWhiteSpace(request.EmailOrUserName) && !isEmail)
                {
                    var query = new GetUserByEmailOrUserName(null!, request.EmailOrUserName);
                    user = await _bus.InvokeAsync<UserDto?>(query);
                    if (user == null)
                        return request.CustomResponse("No user found with the provided username.", HttpStatusCode.NotFound);
                }

                if ((user == null || string.IsNullOrWhiteSpace(user.Password)) || !Encrypt.VerifyPassword(request.Password!, user.Password))
                {
                    return request.CustomResponse("Invalid credentials", HttpStatusCode.Unauthorized);
                }

                if (!string.Equals(user.RoleName, RoleName.Admin, StringComparison.OrdinalIgnoreCase))
                {
                    return request.CustomResponse("The user does not have administrator privileges.", HttpStatusCode.Forbidden);
                }

                var token = Authentication.GenerateToken(new { user.Id, role = user.RoleName }, DateTime.UtcNow.AddDays(1));
                user.RefreshToken = token.RefreshToken;
                user.RefreshTokenExpiryTime = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(7));

                var commands = new UpdateUser(user);
                await _bus.InvokeAsync(commands);
                ManageTokenCookies(token, context);
                return request.CustomResponse("Login successful.", HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return request.CustomResponse(ex.Message, HttpStatusCode.InternalServerError);
            }
        }

        public async Task<ApiResponse> RefreshToken(HttpContext context)
        {
            var response = new ApiResponse();
            try
            {
                // Obtener los tokens desde las cookies
                context.Request.Cookies.TryGetValue("accessToken", out var accessToken);

                if (string.IsNullOrWhiteSpace(accessToken))
                    return response.CustomResponse("Access token or refresh token is empty", HttpStatusCode.Unauthorized);

                var handler = new JwtSecurityTokenHandler();
                var jsonToken = handler.ReadToken(accessToken) as JwtSecurityToken;

                if (jsonToken == null)
                    return response.CustomResponse("Invalid access token", HttpStatusCode.Unauthorized);

                // Obtener el UserId del token
                var userIdClaim = jsonToken.Claims.FirstOrDefault(claim => claim.Type == "Id")?.Value;
                if (string.IsNullOrWhiteSpace(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
                    return response.CustomResponse("UserId not found in token", HttpStatusCode.BadRequest);

                // Buscar usuario en la base de datos
                var query = new GetUserById(userId);
                var user = await _bus.InvokeAsync<UserDto?>(query);
                if (user == null)
                    return response.CustomResponse("User not found", HttpStatusCode.NotFound);

                // Validar el refresh token y su expiración
                if (user.RefreshToken != null && user.RefreshTokenExpiryTime < DateOnly.FromDateTime(DateTime.UtcNow))
                    return response.CustomResponse("Invalid refresh token", HttpStatusCode.Unauthorized);

                // Generar nuevo token
                var token = Authentication.GenerateToken(new { user.Id, role = user.RoleName }, DateTime.UtcNow.AddDays(1));
                if (token == null || string.IsNullOrWhiteSpace(token.RefreshToken))
                    return response.CustomResponse("Error generating authentication token", HttpStatusCode.InternalServerError);

                // Actualizar usuario con el nuevo refresh token
                user.RefreshToken = token.RefreshToken;
                user.RefreshTokenExpiryTime = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(7));

                var commands = new UpdateUser(user);
                await _bus.InvokeAsync(commands);
                ManageTokenCookies(token, context);

                return response.CustomResponse("Token refreshed successfully", HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return response.CustomResponse($"Error during token refresh: {ex.Message}", HttpStatusCode.InternalServerError);
            }
        }

        public ApiResponse Logout(HttpContext context)
        {
            var response = new ApiResponse();
            try
            {
                // Remove the authentication cookies using the same configuration used to create them.
                ManageTokenCookies(null, context, isLogout: true);
                return response.CustomResponse("Logout successful.", HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return response.CustomResponse("An error occurred while trying to log out. Please try again.", HttpStatusCode.InternalServerError);
            }
        }

        private void ManageTokenCookies(Token? tokenDto, HttpContext context, bool isLogout = false)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                Expires = DateTime.Now.AddDays(7)
            };

            if (_environment.IsProduction())
            {
                cookieOptions.SameSite = SameSiteMode.Lax;
                cookieOptions.Domain = "";
                cookieOptions.Path = "/";
            }
            else
            {
                cookieOptions.SameSite = SameSiteMode.None;
            }

            if (isLogout)
            {
                context.Response.Cookies.Delete("accessToken", cookieOptions);
                return;
            }

            context.Response.Cookies.Append("accessToken", tokenDto!.AccessToken, cookieOptions);
        }
    }
}
