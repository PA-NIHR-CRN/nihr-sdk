using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using NIHR.Infrastructure.Models;

namespace NIHR.Infrastructure
{
    public interface IPostcodeMapper
    {
        Task<IEnumerable<PostcodeAddressModel>> GetAddressesByPostcodeAsync(string postcode,
            CancellationToken cancellationToken);

        Task<CoordinatesModel> GetCoordinatesFromPostcodeAsync(string postcode,
            CancellationToken cancellationToken);
    }
}
