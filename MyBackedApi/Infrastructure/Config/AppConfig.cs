using Microsoft.Extensions.Configuration;
using Infrastructure.Config.Models;

namespace Infrastructure.Config
{
    public static class AppConfig
    {
        public static ConnectionStringsSettings ConnectionStrings { get; set; }
        public static JwtSettings JwtSettings { get; set; }

        public static AwsS3Settings AwsS3Settings { get; set; }

        public static SendGridSettings SendGridSettings { get; set; }
        public static ServerSettings ServerSettings { get; set; }
        public static OpenAISettings OpenAISettings { get; set; }
    
        public static void Init(IConfiguration Configuration)
        {
            Configure(Configuration);
        }

        private static void Configure(IConfiguration Configuration)
        {
            var configSection = Configuration.GetSection("ConnectionStrings");
            ConnectionStrings = configSection.Get<ConnectionStringsSettings>();

            var jwtSection = Configuration.GetSection("JWT");
            JwtSettings = jwtSection.Get<JwtSettings>();


            var awsSection = Configuration.GetSection("AWS");
            AwsS3Settings = awsSection.Get<AwsS3Settings>();

            var sendGridSection = Configuration.GetSection("SendGrid");
            SendGridSettings = sendGridSection.Get<SendGridSettings>();

            var serverSection = Configuration.GetSection("Server");
            ServerSettings = serverSection.Get<ServerSettings>();

            var openAiSection = Configuration.GetSection("OpenAI");
            OpenAISettings = openAiSection.Get<OpenAISettings>();
        }
    }
}
