using Logitar.EventSourcing;
using Logitar.Portal.Application.Actors;
using Logitar.Portal.Application.Realms;
using Logitar.Portal.Contracts.Actors;
using Logitar.Portal.Contracts.Realms;
using Logitar.Portal.EntityFrameworkCore.Relational.Actors;
using Logitar.Portal.EntityFrameworkCore.Relational.Entities;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Portal.EntityFrameworkCore.Relational.Queriers;

internal class RealmQuerier : IRealmQuerier
{
  private readonly IActorService _actorService;
  private readonly DbSet<RealmEntity> _realms;

  public RealmQuerier(IActorService actorService, PortalContext context)
  {
    _actorService = actorService;
    _realms = context.Realms;
  }

  public async Task<Realm?> FindAsync(string idOrUniqueSlug, CancellationToken cancellationToken)
  {
    idOrUniqueSlug = idOrUniqueSlug.Trim();
    string uniqueSlugNormalized = idOrUniqueSlug.ToUpper();

    string? aggregateId = null;
    if (Guid.TryParse(idOrUniqueSlug, out Guid id))
    {
      aggregateId = new AggregateId(id).Value;
    }

    RealmEntity[] realms = await _realms.AsNoTracking()
      .Where(x => x.AggregateId == aggregateId || x.UniqueSlugNormalized == uniqueSlugNormalized)
      .ToArrayAsync(cancellationToken);
    if (realms.Length == 0)
    {
      return null;
    }
    RealmEntity realm = realms.Single(realm => realm.AggregateId == aggregateId);

    return (await MapAsync(cancellationToken, realm)).Single();
  }

  public async Task<Realm?> ReadAsync(AggregateId id, CancellationToken cancellationToken)
  {
    RealmEntity? realm = await _realms.AsNoTracking()
      .SingleOrDefaultAsync(x => x.AggregateId == id.Value, cancellationToken);
    if (realm == null)
    {
      return null;
    }

    return (await MapAsync(cancellationToken, realm)).Single();
  }

  private async Task<IEnumerable<Realm>> MapAsync(CancellationToken cancellationToken = default, params RealmEntity[] realms)
  {
    IEnumerable<ActorId> actorIds = realms.SelectMany(realm => realm.GetActorIds()).Distinct();
    Dictionary<ActorId, Actor> actors = await _actorService.FindAsync(actorIds, cancellationToken);
    Mapper mapper = new(actors);

    return realms.Select(mapper.ToRealm);
  }
}
