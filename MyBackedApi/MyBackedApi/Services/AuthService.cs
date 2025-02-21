using Infrastructure.Config;
using Infrastructure.Exceptions;
using Microsoft.IdentityModel.Tokens;
using MyBackedApi.DTOs.Auth.Requests;
using MyBackedApi.DTOs.Auth.Responses;
using MyBackedApi.Enums;
using MyBackedApi.Models;
using MyBackedApi.Repositories;
using MyBackedApi.Services;
using MyBackendApi.Repositories;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MyBackendApi.Services
{
    public class AuthService
    {
        private UserRepository userRepository { get; set; }
        private UserAuthTokenRepository userAuthTokenRepository { get; set; }
        private ActivationCodeRepository activationCodeRepository { get; set; }
        private EmailService emailService { get; set; }

        public AuthService(
            UserRepository userRepository,
            UserAuthTokenRepository userAuthTokenRepository,
            ActivationCodeRepository activationCodeRepository,
            EmailService emailService
            )
        {
            this.userRepository = userRepository;
            this.userAuthTokenRepository = userAuthTokenRepository;
            this.activationCodeRepository = activationCodeRepository;
            this.emailService = emailService;
        }

        public async Task RegisterUserAsync(RegisterUserRequest payload)
        {
            if (!string.IsNullOrWhiteSpace(payload.Email))
            {
                var existingUserByEmail = await userRepository.GetUserByEmailAsync(payload.Email);
                if (existingUserByEmail != null)
                    if (existingUserByEmail.IsApproved == true)
                        throw new EmailAlreadyExistsException("Email already exists");
                    else
                    {
                        if(payload.Role == RoleTypeEnum.STUDENT)
                            await SendActivationCodeAsync(existingUserByEmail.Email);
                        return;
                    }
            }
            var user = new User
            {
                Name = payload.Name,
                Surname = payload.Surname,
                Email = payload.Email,
                Role = payload.Role,
                IsApproved = payload.Role == RoleTypeEnum.COORDINATOR ? true : false
            };
            user.Password = BCrypt.Net.BCrypt.HashPassword(payload.Password, 10);


            if (payload.Role == RoleTypeEnum.STUDENT)
            {
                if(payload.Email.Length <= ("@student.unitbv.com").Length)
                    throw new OperationNotAllowedException("Your email doesn't match the desired behaviour!");

                var userDomain = payload.Email.Substring(payload.Email.Length - ("student.unitbv.com").Length);
                if (userDomain != "student.unitbv.com")
                    throw new OperationNotAllowedException("Your email doesn't match the desired behaviour!");
                user.Occupation = await userRepository.GetOccupationByName("STUDENT");
                user.OccupationId = user.Occupation.Id;
                await SendActivationCodeAsync(user.Email);
            }

            if (payload.Role == RoleTypeEnum.COORDINATOR)
            {
                var userDomain = payload.Email.Substring(payload.Email.Length - ("unitbv.com").Length);
                var possibleStudentDomain = payload.Email.Substring(payload.Email.Length - ("student.unitbv.com").Length - 1);
                if (userDomain != "unitbv.com" || possibleStudentDomain == "student.unitbv.com")
                    throw new OperationNotAllowedException("Your email doesn't match the desired behaviour!");
                user.Occupation = await userRepository.GetOccupationByName("PROFESOR UNITBV");
                user.OccupationId = user.Occupation.Id;
            }

            await userRepository.AddUserAsync(user);
        }

        public async Task SendActivationCodeAsync(string userEmail)
        {
            string activationCode;
            activationCode = await activationCodeRepository.HasActiveValidationCode(userEmail);
            if (string.IsNullOrEmpty(activationCode))
            {
                activationCode = new Random().Next(100000, 999999).ToString(); // Cod de 6 cifre
                await activationCodeRepository.AddActivationCodeAsync(
                    new ActivationCode
                    {
                        Email = userEmail,
                        Code = activationCode,
                        Expiration = DateTime.UtcNow.AddMinutes(5)
                    });
            }

            await emailService.SendActivationEmailAsync(userEmail, activationCode);
        }

        public async Task<bool> VerifyActivationCodeAsync(string email, string code)
        {
            var activationCode = await activationCodeRepository.GetValidActivationCodeAsync(email, code);

            if (activationCode != null)
            {
                await activationCodeRepository.DeleteActivationCodeAsync(activationCode);
                await userRepository.ApproveUserAsync(email);
                return true;
            }

            return false;
        }

        public async Task<LoginResponse> LoginAsync(LoginRequest payload)
        {
            User user = await userRepository.GetActiveUserByEmail(payload.Email);

            VerifyPassword(payload, user);

            var response = new LoginResponse();
            response.AccessToken = GenerateToken(user, true);
            response.RefreshToken = GenerateToken(user, false);

            await SaveRefreshTokenAsync(response.RefreshToken, user.Id);

            return response;
        }

        private void VerifyPassword(LoginRequest payload, User user)
        {
            try
            {
                if (user == null || !BCrypt.Net.BCrypt.Verify(payload.Password, user.Password))
                    throw new UnauthenticatedException("Invalid credentials");
            }
            catch (Exception)
            {
                throw new UnauthenticatedException("Invalid credentials");
            }
        }

        public async Task<RefreshTokenResponse> RefreshTokenAsync(RefreshTokenRequest refreshTokenRequest)
        {
            var token = await userAuthTokenRepository.GetByRefreshTokenAsync(refreshTokenRequest.RefreshToken);

            if (token == null)
                throw new OperationNotAllowedException("Your session is invalid");

            if (token.RefreshTokenExpiryTime < DateTime.UtcNow)
                throw new UnauthenticatedException("Your session has expired");

            if (!token.IsValid || refreshTokenRequest.UserId != token.UserId)
                throw new UnauthenticatedException("Your session is invalid or does not match the user");

            var user = await userRepository.GetUserByIdAsync(refreshTokenRequest.UserId);

            if (user == null)
                throw new OperationNotAllowedException("The user does not exist");

            var response = new RefreshTokenResponse();
            response.AccessToken = GenerateToken(user, true);
            response.RefreshToken = refreshTokenRequest.RefreshToken;

            return response;
        }

        private string GenerateToken(User user, bool isAccessToken = true)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var secret = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(AppConfig.JwtSettings.Secret));

            var roleClaim = new Claim("role", user.Role.ToString());
            var idClaim = new Claim("userId", user.Id.ToString());

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Issuer = AppConfig.JwtSettings.Issuer,
                Audience = AppConfig.JwtSettings.Audience,
                Subject = new ClaimsIdentity(new List<Claim> { roleClaim, idClaim }),
                Expires = isAccessToken ?
                    DateTime.UtcNow.AddMinutes(AppConfig.JwtSettings.AccessTokenValidityMinutes) :
                    DateTime.UtcNow.AddDays(AppConfig.JwtSettings.RefreshTokenValidityDays),
                SigningCredentials = new SigningCredentials(secret, SecurityAlgorithms.HmacSha256)
            };
            SecurityToken securityToken = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(securityToken);
        }

        private async Task SaveRefreshTokenAsync(string refreshToken, Guid userId)
        {
            var userAuthToken = new UserAuthToken();
            userAuthToken.Id = Guid.NewGuid();
            userAuthToken.UserId = userId;
            userAuthToken.RefreshToken = refreshToken;
            userAuthToken.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(AppConfig.JwtSettings.RefreshTokenValidityDays);
            userAuthToken.IsValid = true;

            await userAuthTokenRepository.AddRefreshTokenAsync(userAuthToken);
        }

        public async Task LogoutAsync(LogoutRequest logoutRequest, Guid userId)
        {
            var token = await userAuthTokenRepository.GetByRefreshTokenAsync(logoutRequest.RefreshToken);

            if (token == null)
                throw new ResourceMissingException("The desired token does not exist");

            if (token.UserId != userId)
                throw new UnauthorizedException("You can't log out from this account");

            token.IsValid = false;

            await userAuthTokenRepository.SaveChangesAsync();
        }
    }
}
