using Logitar.Portal.Core.Realms.Models;
using Logitar.Portal.Core.Users.Models;

namespace Logitar.Portal.Core.Users
{
  public interface IUserQuerier
  {
    Task<UserModel?> GetAsync(string username, RealmModel? realm = null, CancellationToken cancellationToken = default);
  }
}
