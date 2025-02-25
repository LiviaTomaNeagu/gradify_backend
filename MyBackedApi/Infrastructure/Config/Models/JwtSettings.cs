namespace Infrastructure.Config.Models
{
    public class JwtSettings
    {
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public string Secret { get; set; }
        public int AccessTokenValidityMinutes { get; set; }
        public int RefreshTokenValidityDays { get; set; }
        public int RefreshTokenValidityMinutes { get; set; }
    }
}