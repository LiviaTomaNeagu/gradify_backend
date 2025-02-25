using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using Infrastructure.Exceptions;

namespace Infrastructure.Base
{
    [ApiController]
    public class BaseApiController : ControllerBase
    {
        public BaseApiController()
        {
        }

        protected Guid GetUserIdFromToken()
        {
            var authorizationHeader = HttpContext.Request.Headers["Authorization"].ToString();
            if (string.IsNullOrEmpty(authorizationHeader) || !authorizationHeader.StartsWith("Bearer "))
                throw new UnauthenticatedException("Missing or invalid authorization header");

            var token = authorizationHeader.Substring("Bearer ".Length).Trim();

            var jwtToken = new JwtSecurityTokenHandler().ReadJwtToken(token);
            if (jwtToken == null)
                throw new UnauthenticatedException("Invalid JWT token");

            var userId = jwtToken.Claims.FirstOrDefault(claim => claim.Type == "userId")?.Value;
            if (userId == null)
                throw new UnauthenticatedException("Invalid JWT token");

            return Guid.Parse(userId);
        }
    }
}
