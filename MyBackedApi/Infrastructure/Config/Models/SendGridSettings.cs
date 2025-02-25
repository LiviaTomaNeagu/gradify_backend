namespace Infrastructure.Config.Models
{
    public class SendGridSettings
    {
        public string SenderEmail { get; set; }
        public string SupportEmail { get; set; }
        public string APIKey { get; set; }
        public string BaseUrl { get; set; }
    }
}
