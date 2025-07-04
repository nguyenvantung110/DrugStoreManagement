using drug_store_api.dtos.Auth;
using drug_store_api.services.IF;
using Microsoft.AspNetCore.Mvc;

namespace drug_store_api.web.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] AuthRequest request)
        {
            var userInfo = await _authService.AuthenticateAsync(request.Username, request.Password);
            if (userInfo?.Token == null)
                return Unauthorized(new { message = "Invalid credentials" });

            return Ok(userInfo);
        }
    }
}
