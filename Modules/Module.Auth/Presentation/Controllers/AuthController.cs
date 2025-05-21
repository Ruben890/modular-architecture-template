using Microsoft.AspNetCore.Mvc;
using Module.Auth.Domain.Interfaces;

namespace Module.Auth.Presentation.Controllers
{
    [ApiVersion("0", Deprecated = true)]
    [ApiVersion("1")]
    [ApiController]
    [Route("api/Auth")]
    internal class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] object jsonIn)
        {
            var login = await _authService.Login(jsonIn);
            return new ObjectResult(login);
        }
    }
}
