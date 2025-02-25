using Amazon.Runtime.Internal.Transform;
using Azure.Core.Serialization;
using Infrastructure.Config;
using SendGrid;
using SendGrid.Helpers.Mail;
using System.Net.Mime;

namespace MyBackedApi.Services
{

    public class EmailService
    {
        private readonly string _apiKey;
        private readonly string _senderEmail;
        private readonly string _senderName;

        public EmailService(IConfiguration configuration)
        {
            _apiKey = AppConfig.SendGridSettings.APIKey;
            _senderEmail = AppConfig.SendGridSettings.SenderEmail;
            _senderName = "Gradify Support Team";
        }

        public async Task SendActivationEmailAsync(string userEmail, string activationCode)
        {
            var client = new SendGridClient(_apiKey);
            var from = new EmailAddress(_senderEmail, _senderName);

            var dynamicData = new Dictionary<string, string>
            {
                {  "ACTIVATION_CODE", activationCode}
            };

            var to = new EmailAddress("liviatoma10@gmail.com");

            var msg = new SendGridMessage
            {
                From = from,
                TemplateId = "d-410be5542790424ab3e19ac8530294ae"
            };

            msg.AddTo(to);
            msg.SetTemplateData(dynamicData);

            var response = await client.SendEmailAsync(msg);
            Console.WriteLine($"Email response status: {response.StatusCode}");
        }

        //public async Task SendActivationEmailAsync(string userEmail, string activationCode)
        //{
        //    var client = new SendGridClient(_apiKey);
        //    var from = new EmailAddress("livia.neagu@cst.ro", _senderName);
        //    var to = new EmailAddress("livia.toma10@gmail.com");

        //    var msg = new SendGridMessage
        //    {
        //        From = from,
        //        Subject = "Account Activation",
        //        PlainTextContent = $"Your activation code is: {activationCode}",
        //        HtmlContent = $"<p>Your activation code is: <strong>{activationCode}</strong></p>"
        //    };

        //    msg.AddTo(to);

        //    var response = await client.SendEmailAsync(msg);
        //}


    }

}
