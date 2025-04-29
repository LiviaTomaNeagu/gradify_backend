namespace Infrastructure.Config.Models
{
    public class AwsS3Settings
    {
        public string Region { get; set; }
        public string AccessKey { get; set; }
        public string SecretKey { get; set; }
        public string BucketName { get; set; }
        public string FolderImages { get; set; }
        public string FolderDocuments { get; set; }
    }
}
