namespace Infrastructure.Config.Models
{
    public class AwsSettings
    {
        public string Profile { get; set; }
        public string Region { get; set; }
        public string Key { get; set; }
        public string Secret { get; set; }
        public string ProfileImagePath { get; set; }
        public string Trails { get; set; }
        public string TrailsImagePath { get; set; }
    }
}
