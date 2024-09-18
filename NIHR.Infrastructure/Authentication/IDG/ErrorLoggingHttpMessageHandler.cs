using Microsoft.Extensions.Logging;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NIHR.Infrastructure.Authentication.IDG
{
    public class ErrorLoggingHttpMessageHandler : DelegatingHandler
    {
        private readonly ILogger<ErrorLoggingHttpMessageHandler> _logger;

        public ErrorLoggingHttpMessageHandler(ILogger<ErrorLoggingHttpMessageHandler> logger)
        {
            _logger = logger;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var response = await base.SendAsync(request, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                await response.Content.LoadIntoBufferAsync();
                var stream = (MemoryStream)await response.Content.ReadAsStreamAsync();
                stream.Seek(0, SeekOrigin.Begin);

                using var sr = new StreamReader(stream, Encoding.UTF8, detectEncodingFromByteOrderMarks: true, bufferSize: 1024, leaveOpen: true);
                var errorContent = await sr.ReadToEndAsync();
                stream.Seek(0, SeekOrigin.Begin);

                _logger.LogError($"{request.Method} {request.RequestUri} {response.StatusCode}: {errorContent}");
            }

            return response;
        }
    }
}