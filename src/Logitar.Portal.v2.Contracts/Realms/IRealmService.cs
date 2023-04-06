namespace Logitar.Portal.v2.Contracts.Realms;

public interface IRealmService
{
  Task<Realm> CreateAsync(CreateRealmInput input, CancellationToken cancellationToken = default);
  Task<Realm> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
  // Task<RealmModel> GetAsync(string id, CancellationToken cancellationToken = default);
  // Task<ListModel<RealmSummary>> GetAsync(string? search = null, RealmSort? sort = null, bool desc = false, int? index = null, int? count = null, CancellationToken cancellationToken = default);
  Task<Realm> UpdateAsync(Guid id, UpdateRealmInput input, CancellationToken cancellationToken = default);
}
