using Logitar.EventSourcing;
using Logitar.Identity.EntityFrameworkCore.Relational.IdentityDb;
using Logitar.Portal.Domain.Realms;
using Logitar.Portal.EntityFrameworkCore.Relational.Entities;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Portal.EntityFrameworkCore.Relational.Repositories;

internal class RealmRepository : Repository, IRealmRepository
{
  private readonly DbSet<RealmEntity> _realms;

  public RealmRepository(PortalContext context, IEventStore eventStore) : base(eventStore)
  {
    _realms = context.Realms;
  }

  public async Task<IReadOnlyCollection<Realm>> LoadAsync(CancellationToken cancellationToken)
  {
    return await LoadAsync<Realm>(cancellationToken);
  }

  public async Task<Realm?> LoadAsync(RealmId id, CancellationToken cancellationToken)
  {
    return await LoadAsync(id, version: null, cancellationToken);
  }
  public async Task<Realm?> LoadAsync(RealmId id, long? version, CancellationToken cancellationToken)
  {
    return await LoadAsync<Realm>(id.StreamId, version, cancellationToken);
  }

  public async Task<Realm?> LoadAsync(Slug uniqueSlug, CancellationToken cancellationToken)
  {
    string uniqueSlugNormalized = Helper.Normalize(uniqueSlug.Value);

    string? streamId = await _realms.AsNoTracking()
      .Where(x => x.UniqueSlugNormalized == uniqueSlugNormalized)
      .Select(x => x.StreamId)
      .SingleOrDefaultAsync(cancellationToken);
    if (streamId == null)
    {
      return null;
    }

    return await LoadAsync<Realm>(new StreamId(streamId), cancellationToken);
  }

  public async Task SaveAsync(Realm realm, CancellationToken cancellationToken)
  {
    await base.SaveAsync(realm, cancellationToken);
  }
  public async Task SaveAsync(IEnumerable<Realm> realms, CancellationToken cancellationToken)
  {
    await base.SaveAsync(realms, cancellationToken);
  }
}
