namespace MyBackedApi.Models
{
    public class UserAuthToken : BaseModel
    {
        public string RefreshToken { get; set; }
        public DateTime RefreshTokenExpiryTime { get; set; }
        public bool IsValid { get; set; }
        public Guid UserId { get; set; }
        public User User { get; set; }

    }
}
