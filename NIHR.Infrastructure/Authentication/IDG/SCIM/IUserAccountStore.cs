using System.Threading;
using System.Threading.Tasks;

namespace NIHR.Infrastructure.Authentication.IDG.SCIM
{
    public interface IUserAccountStore
    {
        Task<string> CreateNewUserAsync(string email, string firstName, string lastName, string password, CancellationToken token);
        Task<bool> UserWithEmailExistsAsync(string email, CancellationToken token);
    }
}