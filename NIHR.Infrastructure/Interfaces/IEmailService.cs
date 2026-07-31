using System.Threading;
using System.Threading.Tasks;
using NIHR.Infrastructure.Models;

namespace NIHR.Infrastructure.Interfaces
{
    public interface IEmailService
    {
        Task SendEmailAsync(string to, string subject, string body, CancellationToken cancellationToken = default);
        
        Task<SendEmailResult> SendEmailWithResultAsync(string to, string subject, string body, CancellationToken cancellationToken = default);
    }
}
