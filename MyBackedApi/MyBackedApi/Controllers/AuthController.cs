using Infrastructure.Base;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyBackedApi.DTOs.Auth.Requests;
using MyBackedApi.DTOs.Auth.Responses;
using MyBackendApi.Services;

namespace MyBackendApi.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : BaseApiController
    {
        private AuthService authService { get; set; }

        public AuthController(AuthService authService)
        {
            this.authService = authService;
        }

        [HttpPost("login")]
        public async Task<ActionResult<LoginResponse>> Login(LoginRequest payload)
        {
            var response = await authService.LoginAsync(payload);

            return Ok(response);
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken(RefreshTokenRequest refreshTokenRequest)
        {
            //TODO TEST
            var tokenResponse = await authService.RefreshTokenAsync(refreshTokenRequest);

            return Ok(tokenResponse);
        }

        [Authorize]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout(LogoutRequest logoutRequest)
        {
            var currentUserId = GetUserIdFromToken();

            await authService.LogoutAsync(logoutRequest, currentUserId);

            return Ok(new BaseResponseEmpty { Message = "Successfully logged out" });
        }


    }
}
