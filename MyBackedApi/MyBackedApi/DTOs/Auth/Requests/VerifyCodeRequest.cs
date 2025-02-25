namespace MyBackedApi.DTOs.Auth.Requests
{
    public class VerifyCodeRequest
    {
        public string Email { get; set; }
        public string Code { get; set; }
    }
}
