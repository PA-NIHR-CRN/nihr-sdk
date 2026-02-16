using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using NIHR.Infrastructure.Models;

namespace NIHR.Infrastructure.Interfaces
{
    public interface IContentProvider
    {
        Task<TContent> GetContentAsync<TContent>(ContentRequestModel contentRequest, CancellationToken cancellationToken = default) where TContent : new();
        Task<(List<TContent>, int)> GetContentAsListWithTotalAsync<TContent>(ContentRequestModel contentRequest, CancellationToken cancellationToken = default) where TContent : new();
 
    }
}
