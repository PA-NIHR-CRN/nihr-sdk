using System.Threading;
using System.Threading.Tasks;

namespace NIHR.Infrastructure.Interfaces
{
    public interface IContentProvider
    {
        Task<TContent> GetContentAsync<TContent>(string contentId, CancellationToken cancellationToken = default) where TContent : new();

        Task<TContent> GetContentAsync<TContent>(string contentId, string contentType, CancellationToken cancellationToken = default) where TContent : new();
    }
}
