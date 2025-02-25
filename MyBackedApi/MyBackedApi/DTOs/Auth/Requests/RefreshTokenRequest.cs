namespace MyBackedApi.DTOs.Auth.Requests
{
    public class RefreshTokenRequest
    {
        public string RefreshToken { get; set; }
        public Guid UserId { get; set; }

    }
}
