using Infrastructure.Base;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyBackedApi.DTOs.Auth.Requests;
using MyBackedApi.DTOs.Auth.Responses;
using MyBackedApi.Models;
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

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterUserRequest payload)
        {
           await authService.RegisterUserAsync(payload);

            return Ok(new BaseResponseEmpty { Message = "Successfully registered" });
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken(RefreshTokenRequest refreshTokenRequest)
        {
            //TODO TEST
            var tokenResponse = await authService.RefreshTokenAsync(refreshTokenRequest);

            return Ok(tokenResponse);
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout(LogoutRequest logoutRequest)
        {
            var currentUserId = GetUserIdFromToken();

            await authService.LogoutAsync(logoutRequest, currentUserId);

            return Ok(new BaseResponseEmpty { Message = "Successfully logged out" });
        }

        [HttpPost("verify-activation-code")]
        public async Task<IActionResult> VerifyActivationCode([FromBody] VerifyCodeRequest request)
        {
            if (await authService.VerifyActivationCodeAsync(request.Email, request.Code))
            {
                return Ok(new { Message = "Activation successful. You can now log in." });
            }
            return BadRequest(new { Message = "Invalid activation code." });
        }

        [HttpPost("resend-activation-code")]
        public async Task<IActionResult> ResendActivationCode([FromBody] string email)
        {
            await authService.SendActivationCodeAsync(email);

            return Ok(new BaseResponseEmpty { Message = "Successfully registered" });
        }



    }
}
