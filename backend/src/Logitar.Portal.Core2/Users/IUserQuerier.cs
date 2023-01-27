using Logitar.Portal.Core2.Realms.Models;
using Logitar.Portal.Core2.Users.Models;

namespace Logitar.Portal.Core2.Users
{
  public interface IUserQuerier
  {
    Task<UserModel?> GetAsync(string username, RealmModel? realm = null, CancellationToken cancellationToken = default);
  }
}
