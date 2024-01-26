using Logitar.EventSourcing;
using Logitar.Portal.Application.Realms;
using Logitar.Portal.Contracts.Actors;
using Logitar.Portal.Contracts.Realms;
using Logitar.Portal.Domain.Realms;
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

  public async Task<Realm> ReadAsync(RealmAggregate realm, CancellationToken cancellationToken)
  {
    return await ReadAsync(realm.Id, cancellationToken)
      ?? throw new InvalidOperationException($"The realm entity 'AggregateId={realm.Id.Value}' could not be found.");
  }
  public async Task<Realm?> ReadAsync(RealmId id, CancellationToken cancellationToken)
    => await ReadAsync(id.Value, cancellationToken);
  public async Task<Realm?> ReadAsync(string id, CancellationToken cancellationToken)
  {
    string aggregateId = id.Trim();

    RealmEntity? realm = await _realms.AsNoTracking()
      .SingleOrDefaultAsync(x => x.AggregateId == aggregateId, cancellationToken);

    if (realm == null)
    {
      return null;
    }

    return await MapAsync(realm, cancellationToken);
  }

  public async Task<Realm?> ReadByUniqueSlugAsync(string uniqueSlug, CancellationToken cancellationToken)
  {
    string uniqueSlugNormalized = uniqueSlug.Trim().ToUpper();

    RealmEntity? realm = await _realms.AsNoTracking()
      .SingleOrDefaultAsync(x => x.UniqueSlugNormalized == uniqueSlugNormalized, cancellationToken);
    if (realm == null)
    {
      return null;
    }

    return await MapAsync(realm, cancellationToken);
  }

  private async Task<Realm> MapAsync(RealmEntity realm, CancellationToken cancellationToken)
  => (await MapAsync([realm], cancellationToken)).Single();
  private async Task<IEnumerable<Realm>> MapAsync(IEnumerable<RealmEntity> realms, CancellationToken cancellationToken)
  {
    IEnumerable<ActorId> actorIds = realms.SelectMany(realm => realm.GetActorIds());
    IEnumerable<Actor> actors = await _actorService.FindAsync(actorIds, cancellationToken);
    Mapper mapper = new(actors);

    return realms.Select(mapper.ToRealm);
  }
}
