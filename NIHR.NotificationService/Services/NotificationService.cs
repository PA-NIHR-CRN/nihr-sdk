using System.Diagnostics;
using Microsoft.Extensions.Logging;
using NIHR.NotificationService.Context;
using NIHR.NotificationService.Interfaces;
using NIHR.NotificationService.Models;
using Notify.Client;
using Notify.Models.Responses;
using Polly;
using Polly.RateLimit;

namespace NIHR.NotificationService.Services
{
    public class NotificationService : INotificationService
    {
        private readonly NotificationClient _client;
        private readonly ILogger<NotificationService> _logger;
        private static readonly SemaphoreSlim _semaphore = new(1, 1);
        private static int _dailyEmailCount = 0;
        private const int _dailyLimit = 250000;
        private const int _rateLimitPerMinute = 3000;

        public NotificationService(NotificationClient client, ILogger<NotificationService> logger)
        {
            _client = client;
            _logger = logger;
        }

        private async Task SendEmailAsync(SendEmailRequest request, CancellationToken cancellationToken)
        {
            var stopwatch = Stopwatch.StartNew();
            try
            {
                var personalisation = request.Personalisation.ToDictionary(x => x.Key, x => (dynamic)x.Value);
                    
                await _client.SendEmailAsync(request.EmailAddress, request.EmailTemplateId,
                    personalisation, request.Reference);
                await IncrementDailyEmailCountAsync(1);
            }
            catch (HttpRequestException httpEx) when (httpEx.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
            {
                _logger.LogError(httpEx, "429 Rate Limit Exceeded error while sending email");
                throw new InvalidOperationException("429 Rate Limit Exceeded error while sending email.", httpEx);
            }
            finally
            {
                stopwatch.Stop();
                _logger.LogInformation("Request for {EmailAddress} took {ElapsedMilliseconds} ms", request.EmailAddress, stopwatch.ElapsedMilliseconds);
            }
        }

        public async Task SendPreviewEmailAsync(SendEmailRequest request, CancellationToken cancellationToken)
        {
            var personalisation = request.Personalisation.ToDictionary(x => x.Key, x => (dynamic)x.Value);

            await _client.SendEmailAsync(request.EmailAddress, request.EmailTemplateId,
                personalisation, request.Reference);
        }

        public async Task<EmailNotificationResponse> SendBatchEmailAsync(List<Notification> notifications,
            CancellationToken cancellationToken)
        {
            const int batchSize = 100;

            var rateLimitPolicy = Policy.RateLimitAsync(_rateLimitPerMinute, TimeSpan.FromMinutes(1));
            var retryPolicy = Policy
                .Handle<HttpRequestException>(ex => ex.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
                .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(20));

            var tasks = new List<Task>();
            var batches = notifications
                .Select((notification, index) => new { Index = index, Value = notification })
                .GroupBy(item => item.Index / batchSize, item => item.Value);

            var totalStopwatch = Stopwatch.StartNew();

            foreach (var batch in batches)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    _logger.LogWarning("Batch email sending cancelled");
                    cancellationToken.ThrowIfCancellationRequested();
                }

                tasks.Add(SendBatchWithRateLimitAsync(batch.ToList(), rateLimitPolicy, retryPolicy, cancellationToken));
            }

            await Task.WhenAll(tasks);

            totalStopwatch.Stop();
            _logger.LogInformation("Total time for all batches: {TotalMilliseconds} ms", totalStopwatch.ElapsedMilliseconds);

            return new EmailNotificationResponse();
        }

        private async Task SendBatchWithRateLimitAsync(List<Notification> batch,
            AsyncRateLimitPolicy rateLimitPolicy, AsyncPolicy retryPolicy, CancellationToken cancellationToken)
        {
            var batchStopwatch = Stopwatch.StartNew();
            var individualTimes = new List<long>();

            var tasks = batch.Select(notification => 
                SendEmailWithRetryAsync(notification, retryPolicy, cancellationToken, individualTimes)).ToList();

            await rateLimitPolicy.ExecuteAsync(() => Task.WhenAll(tasks));

            batchStopwatch.Stop();
            var averageTimePerRequest = individualTimes.Average();
            _logger.LogInformation("Batch of {BatchCount} emails took {ElapsedMilliseconds} ms. Average time per request: {AverageTimePerRequest} ms",
                batch.Count, batchStopwatch.ElapsedMilliseconds, averageTimePerRequest);
        }

        private async Task SendEmailWithRetryAsync(Notification notification,
            AsyncPolicy retryPolicy, CancellationToken cancellationToken, List<long> individualTimes)
        {
            var personalisation = notification.NotificationDatas.ToDictionary(x => x.Key, x => x.Value);
            var email = personalisation["email"];
            var sendEmailRequest = CreateSendEmailRequest(email, personalisation);

            var stopwatch = Stopwatch.StartNew();
            await retryPolicy.ExecuteAsync(async () => { await SendEmailAsync(sendEmailRequest, cancellationToken); });
            stopwatch.Stop();
            lock (individualTimes)
            {
                individualTimes.Add(stopwatch.ElapsedMilliseconds);
            }
        }

        private static SendEmailRequest CreateSendEmailRequest(string email, Dictionary<string, string> personalisation)
        {
            if (!personalisation.TryGetValue("emailCampaignParticipantId", out var reference))
            {
                throw new KeyNotFoundException($"EmailCampaignParticipantId not found for email: {email}");
            }
            
            if (!personalisation.TryGetValue("emailTemplateId", out var emailTemplateId))
            {
                throw new KeyNotFoundException($"EmailTemplateId not found for email: {email}");
            }

            return new SendEmailRequest
            {
                EmailAddress = email,
                EmailTemplateId = emailTemplateId,
                Personalisation = personalisation,
                Reference = reference
            };
        }

        private async Task IncrementDailyEmailCountAsync(int count)
        {
            var lockStopwatch = Stopwatch.StartNew();
            await _semaphore.WaitAsync();
            lockStopwatch.Stop();
            try
            {
                _dailyEmailCount += count;
                if (_dailyEmailCount >= _dailyLimit)
                {
                    throw new InvalidOperationException("Daily email limit reached.");
                }
            }
            finally
            {
                _semaphore.Release();
                _logger.LogInformation("Lock duration for IncrementDailyEmailCountAsync: {ElapsedMilliseconds} ms", lockStopwatch.ElapsedMilliseconds);
            }
        }

        public async Task<TemplateList> GetTemplatesAsync(CancellationToken cancellationToken)
        {
            return await _client.GetAllTemplatesAsync();
        }
    }
}
