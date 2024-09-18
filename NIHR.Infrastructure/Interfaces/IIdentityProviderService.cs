using System.Threading;
using System.Threading.Tasks;

namespace NIHR.Infrastructure.Interfaces
{
    public interface IIdentityProviderService
    {
        Task<string> GetOrAcquireTokenAsync(CancellationToken cancellationToken = default);
    }
}
