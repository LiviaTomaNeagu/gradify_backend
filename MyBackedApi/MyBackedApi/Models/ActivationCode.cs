namespace MyBackedApi.Models
{
    public class ActivationCode : BaseModel
    {
        public string Code { get; set; }
        public string Email { get; set; }
        public DateTime Expiration { get; set; }
    }
}
