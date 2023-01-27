using Logitar.Portal.Core.Models;
using Logitar.Portal.Core.Realms.Models;

namespace Logitar.Portal.Core.Realms
{
  public interface IRealmQuerier
  {
    Task<RealmModel?> GetAsync(string idOrAlias, CancellationToken cancellationToken = default);
    Task<ListModel<RealmModel>> GetPagedAsync(string? search = null,
      RealmSort? sort = null, bool isDescending = false,
      int? index = null, int? count = null,
      CancellationToken cancellationToken = default);
  }
}
