using Logitar.Portal.Domain.Realms;
using Logitar.Portal.Domain.Users;

namespace Logitar.Portal.Application.Users
{
  public interface IUserRepository : IRepository
  {
    Task<IEnumerable<User>> LoadByEmailAsync(string email, Realm? realm = null, CancellationToken cancellationToken = default);
    Task<User?> LoadByExternalProviderAsync(Realm realm, string key, string value, CancellationToken cancellationToken = default);
    Task<User?> LoadByUsernameAsync(string username, Realm? realm = null, CancellationToken cancellationToken = default);
  }
}
