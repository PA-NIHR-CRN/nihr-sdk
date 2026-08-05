using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Amazon.SimpleEmail;
using Amazon.SimpleEmail.Model;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NIHR.Infrastructure.Interfaces;
using NIHR.Infrastructure.Models;
using NIHR.Infrastructure.Settings;

namespace NIHR.Infrastructure.Services
{
    public class EmailService : IEmailService
    {
        private readonly IOptions<EmailSettings> _emailSettings;
        private readonly IAmazonSimpleEmailService _client;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IOptions<EmailSettings> emailSettings, IAmazonSimpleEmailService client, ILogger<EmailService> logger)
        {
            _emailSettings = emailSettings;
            _client = client;
            _logger = logger;
        }

        public async Task SendEmailAsync(string to, string subject, string body, CancellationToken cancellationToken = default)
        {
            await SendEmailWithResultAsync(to, subject, body, cancellationToken);
        }
        
        public async Task<SendEmailResult> SendEmailWithResultAsync(string to, string subject, string body, CancellationToken cancellationToken = default)
        {
            var from = _emailSettings.Value.FromAddress;
            var sourceArn = _emailSettings.Value.SourceArn;
            
            _logger.LogInformation("SES Config - FromAddress: '{From}', SourceArn: '{SourceArn}'", from, sourceArn);
            
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

            if (!string.IsNullOrWhiteSpace(sourceArn))
            {
                request.SourceArn = sourceArn;
                request.ReturnPathArn = sourceArn;
                _logger.LogInformation("SES SourceArn set on request: '{SourceArn}'", request.SourceArn);
            }
            else
            {
                _logger.LogWarning("SES SourceArn not set");
            }

            var response = await _client.SendEmailAsync(request, cancellationToken);

            return new SendEmailResult
            {
                MessageId = response.MessageId
            };
        }
    }
}
