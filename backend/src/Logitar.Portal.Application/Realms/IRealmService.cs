using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Realms;

namespace Logitar.Portal.Application.Realms
{
  public interface IRealmService
  {
    Task<RealmModel> CreateAsync(CreateRealmPayload payload, CancellationToken cancellationToken = default);
    Task DeleteAsync(string id, CancellationToken cancellationToken = default);
    Task<RealmModel?> GetAsync(string id, CancellationToken cancellationToken = default);
    Task<ListModel<RealmModel>> GetAsync(string? search = null, RealmSort? sort = null, bool isDescending = false, int? index = null, int? count = null, CancellationToken cancellationToken = default);
    Task<RealmModel> UpdateAsync(string id, UpdateRealmPayload payload, CancellationToken cancellationToken = default);
  }
}
