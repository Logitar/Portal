namespace Logitar.Portal.v2.Contracts.Realms;

public interface IRealmService
{
  Task<Realm> CreateAsync(CreateRealmInput input, CancellationToken cancellationToken = default);
  Task<Realm> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
  Task<Realm?> GetAsync(string idOrUniqueName, CancellationToken cancellationToken = default);
  Task<PagedList<Realm>> GetAsync(string? search = null, RealmSort? sort = null, bool isDescending = false,
    int? skip = null, int? limit = null, CancellationToken cancellationToken = default);
  // TODO(fpion): renamed 'isDescending', 'skip' and 'limit'
  Task<Realm> UpdateAsync(Guid id, UpdateRealmInput input, CancellationToken cancellationToken = default);
}
