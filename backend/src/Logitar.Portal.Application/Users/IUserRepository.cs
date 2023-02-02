using Logitar.Portal.Domain.Realms;
using Logitar.Portal.Domain.Users;

namespace Logitar.Portal.Application.Users
{
  public interface IUserRepository : IRepository
  {
    Task<User?> LoadByUsernameAsync(string username, Realm? realm = null, CancellationToken cancellationToken = default);
  }
}
