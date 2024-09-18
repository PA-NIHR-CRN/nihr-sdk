using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Amazon.SimpleEmail;
using Amazon.SimpleEmail.Model;
using Microsoft.Extensions.Options;
using NIHR.Infrastructure.Interfaces;
using NIHR.Infrastructure.Settings;

namespace NIHR.Infrastructure.Services
{
    public class EmailService : IEmailService
    {
        private readonly IOptions<EmailSettings> _emailSettings;
        private readonly IAmazonSimpleEmailService _client;

        public EmailService(IOptions<EmailSettings> emailSettings, IAmazonSimpleEmailService client)
        {
            _emailSettings = emailSettings;
            _client = client;
        }

        public async Task SendEmailAsync(string to, string subject, string body, CancellationToken cancellationToken = default)
        {
            var from = _emailSettings.Value.FromAddress;
            var request = new SendEmailRequest
            {
                Source = from,
                Destination = new Destination
                {
                    ToAddresses = new List<string> { to }
                },
                Message = new Message
                {
                    Subject = new Content(subject),
                    Body = new Body
                    {
                        Html = new Content(body)
                    }
                }
            };

            await _client.SendEmailAsync(request, cancellationToken);
        }
    }
}
