using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Realms;
using Logitar.Portal.Domain;

namespace Logitar.Portal.Application.Realms
{
  public interface IRealmQuerier
  {
    Task<RealmModel?> GetAsync(AggregateId id, CancellationToken cancellationToken = default);
    Task<RealmModel?> GetAsync(string id, CancellationToken cancellationToken = default);
    Task<ListModel<RealmModel>> GetPagedAsync(string? search = null,
      RealmSort? sort = null, bool isDescending = false,
      int? index = null, int? count = null,
      CancellationToken cancellationToken = default);
  }
}
