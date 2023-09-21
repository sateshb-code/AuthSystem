using Azure.Core;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Options;
using RestSharp;
using RestSharp.Authenticators;

namespace AuthSystem.Services
{
    public class EmailSender : IEmailSender
    {
        private readonly ILogger _logger;
        public AuthSystemSenderOptions Options { get; }

        public EmailSender(IOptions<AuthSystemSenderOptions> optionsAccessor, ILogger<EmailSender> logger)
        {
            _logger = logger;
            Options = optionsAccessor.Value;
        }
        async Task IEmailSender.SendEmailAsync(string email, string subject, string htmlMessage)
        {
            if(string .IsNullOrEmpty(Options.MailgunKey))
            {
                throw new Exception("Null MailgunKey");
            }
            await Execute(Options.MailgunKey, subject, htmlMessage, email);
        }

        public async Task Execute(string apiKey, string subject, string message, string toEmail)
        {
            string baseUrl = "https://api.mailgun.net/v3";
            var options = new RestClientOptions();

            options.BaseUrl = new Uri(baseUrl);
            options.Authenticator = new HttpBasicAuthenticator("api", apiKey);

            RestClient client = new RestClient(options);
            RestRequest request = new RestRequest();

            request.AddParameter("domain", "sandbox71d13dcd889e4fc5bf276f4150f70dd4.mailgun.org", ParameterType.UrlSegment);
            request.Resource = "{domain}/messages";
            request.AddParameter("from", "SabalLtd <sateshb@hotmail.com>");
            request.AddParameter("to", toEmail);
            request.AddParameter("subject", subject);
            request.AddParameter("text", message);
            request.Method = Method.Post;
            var response = await client.ExecuteAsync(request);

            _logger.LogInformation(response.IsSuccessStatusCode ? $"Email to {toEmail} queued successfully!" 
                                                                    : $"Failure Email to {toEmail}");
        }

        
    }
}
