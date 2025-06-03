using Microsoft.AspNetCore.Mvc;
using Module.Auth.Domain.Interfaces;
using Shared.DTO.Request.Dtos;

namespace Module.Auth.Presentation.Controllers
{
    [ApiVersion("0", Deprecated = true)]
    [ApiVersion("1")]
    [ApiController]
    [Route("api/[controller]")]
    internal class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] RequestLogin request) =>
        new ObjectResult(await _authService.Login(request, HttpContext));


        [HttpPost("Logout")]
        public IActionResult Logout() =>
         new ObjectResult(_authService.Logout(HttpContext));


        [HttpPost("RefreshToken")]
        public async Task<IActionResult> RefreshToken() =>
          new ObjectResult(await _authService.RefreshToken(HttpContext));
    }
}
