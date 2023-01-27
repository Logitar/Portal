using Logitar.Portal.Core.Realms.Models;

namespace Logitar.Portal.Core.Realms
{
  public interface IRealmQuerier
  {
    Task<RealmModel?> GetAsync(string aggregateIdOrAlias, CancellationToken cancellationToken = default);
  }
}
