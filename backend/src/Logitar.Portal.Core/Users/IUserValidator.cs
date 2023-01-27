using Logitar.Portal.Core.Configurations;
using Logitar.Portal.Core.Realms;

namespace Logitar.Portal.Core.Users
{
  public interface IUserValidator
  {
    void ValidateAndThrow(User user, Configuration configuration);
    void ValidateAndThrow(User user, Realm realm);

    Task ValidateAndThrowAsync(User user, CancellationToken cancellationToken = default);
  }
}
