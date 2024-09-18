using System.Threading;
using System.Threading.Tasks;

namespace NIHR.Infrastructure.Authentication.IDG.SCIM
{
    public class NullUserAccountStore : IUserAccountStore
    {
        public Task<string> CreateNewUserAsync(string email, string firstName, string lastName, string password, CancellationToken token)
        {
            return Task.FromResult(string.Empty);
        }

        public Task<bool> UserWithEmailExistsAsync(string email, CancellationToken token)
        {
            return Task.FromResult(false);
        }
    }
}