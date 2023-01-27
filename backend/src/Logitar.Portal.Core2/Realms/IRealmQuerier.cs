using Logitar.Portal.Core2.Realms.Models;

namespace Logitar.Portal.Core2.Realms
{
  public interface IRealmQuerier
  {
    Task<RealmModel?> GetAsync(string aggregateIdOrAlias, CancellationToken cancellationToken = default);
  }
}
