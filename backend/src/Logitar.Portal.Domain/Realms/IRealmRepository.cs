namespace Logitar.Portal.Domain.Realms;

public interface IRealmRepository
{
  Task<IEnumerable<Realm>> LoadAsync(CancellationToken cancellationToken = default);
  Task<Realm?> LoadAsync(Guid id, CancellationToken cancellationToken = default);
  Task<Realm?> LoadAsync(RealmId id, CancellationToken cancellationToken = default);
  Task<Realm?> LoadAsync(RealmId id, long? version, CancellationToken cancellationToken = default);
  Task<Realm?> LoadAsync(Slug uniqueSlug, CancellationToken cancellationToken = default);

  Task SaveAsync(Realm realm, CancellationToken cancellationToken = default);
  Task SaveAsync(IEnumerable<Realm> realms, CancellationToken cancellationToken = default);
}
