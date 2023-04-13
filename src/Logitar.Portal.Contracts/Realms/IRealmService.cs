namespace Logitar.Portal.Contracts.Realms;

public interface IRealmService
{
  Task<Realm> CreateAsync(CreateRealmInput input, CancellationToken cancellationToken = default);
  Task<Realm> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
  Task<Realm?> GetAsync(Guid? id = null, string? uniqueName = null, CancellationToken cancellationToken = default);
  Task<PagedList<Realm>> GetAsync(string? search = null, RealmSort? sort = null, bool isDescending = false,
    int? skip = null, int? limit = null, CancellationToken cancellationToken = default);
  Task<Realm> UpdateAsync(Guid id, UpdateRealmInput input, CancellationToken cancellationToken = default);
}
