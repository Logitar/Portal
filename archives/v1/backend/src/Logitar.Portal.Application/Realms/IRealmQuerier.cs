using Logitar.Portal.Core.Realms;
using Logitar.Portal.Domain.Realms;

namespace Logitar.Portal.Application.Realms
{
  public interface IRealmQuerier
  {
    Task<Realm?> GetAsync(string key, bool readOnly = false, CancellationToken cancellationToken = default);
    Task<Realm?> GetAsync(Guid id, bool readOnly = false, CancellationToken cancellationToken = default);
    Task<Realm?> GetByAliasAsync(string alias, bool readOnly = false, CancellationToken cancellationToken = default);
    Task<PagedList<Realm>> GetPagedAsync(string? search = null,
      RealmSort? sort = null, bool desc = false,
      int? index = null, int? count = null,
      bool readOnly = false, CancellationToken cancellationToken = default);
  }
}
