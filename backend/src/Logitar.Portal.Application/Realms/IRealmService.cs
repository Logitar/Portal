using Logitar.Portal.Core;
using Logitar.Portal.Core.Realms;
using Logitar.Portal.Core.Realms.Models;
using Logitar.Portal.Core.Realms.Payloads;

namespace Logitar.Portal.Application.Realms
{
  public interface IRealmService
  {
    Task<RealmModel> CreateAsync(CreateRealmPayload payload, CancellationToken cancellationToken = default);
    Task<RealmModel> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    Task<RealmModel?> GetAsync(string id, CancellationToken cancellationToken = default);
    Task<ListModel<RealmModel>> GetAsync(string? search = null,
      RealmSort? sort = null, bool desc = false,
      int? index = null, int? count = null,
      CancellationToken cancellationToken = default);
    Task<RealmModel> UpdateAsync(Guid id, UpdateRealmPayload payload, CancellationToken cancellationToken = default);
  }
}
