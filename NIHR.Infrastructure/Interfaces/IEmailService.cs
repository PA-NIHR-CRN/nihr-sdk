using System.Threading;
using System.Threading.Tasks;
using NIHR.Infrastructure.Models;

namespace NIHR.Infrastructure.Interfaces
{
    public interface IEmailService
    {
        Task<SendEmailResult> SendEmailAsync(string to, string subject, string body, CancellationToken cancellationToken = default);
    }
}
