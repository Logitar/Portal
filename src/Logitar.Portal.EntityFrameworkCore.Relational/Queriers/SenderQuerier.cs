using Logitar.Data;
using Logitar.EventSourcing;
using Logitar.Portal.Application.Actors;
using Logitar.Portal.Application.Realms;
using Logitar.Portal.Application.Senders;
using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Actors;
using Logitar.Portal.Contracts.Realms;
using Logitar.Portal.Contracts.Senders;
using Logitar.Portal.Domain.Senders;
using Logitar.Portal.EntityFrameworkCore.Relational.Actors;
using Logitar.Portal.EntityFrameworkCore.Relational.Entities;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Portal.EntityFrameworkCore.Relational.Queriers;

internal class SenderQuerier : ISenderQuerier
{
  private readonly IActorService _actorService;
  private readonly IRealmQuerier _realmQuerier;
  private readonly DbSet<SenderEntity> _senders;
  private readonly ISqlHelper _sqlHelper;

  public SenderQuerier(IActorService actorService, PortalContext context, IRealmQuerier realmQuerier, ISqlHelper sqlHelper)
  {
    _actorService = actorService;
    _realmQuerier = realmQuerier;
    _senders = context.Senders;
    _sqlHelper = sqlHelper;
  }

  public async Task<Sender> ReadAsync(SenderAggregate sender, CancellationToken cancellationToken)
  {
    return await ReadAsync(sender.Id, cancellationToken)
      ?? throw new EntityNotFoundException<SenderEntity>(sender.Id);
  }
  public async Task<Sender?> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    return await ReadAsync(new AggregateId(id), cancellationToken);
  }
  private async Task<Sender?> ReadAsync(AggregateId id, CancellationToken cancellationToken)
  {
    string aggregateId = id.Value;

    SenderEntity? sender = await _senders.AsNoTracking()
      .SingleOrDefaultAsync(x => x.AggregateId == aggregateId, cancellationToken);
    if (sender == null)
    {
      return null;
    }

    Realm? realm = null;
    if (sender.TenantId != null)
    {
      AggregateId realmId = new(sender.TenantId);
      realm = await _realmQuerier.ReadAsync(realmId, cancellationToken)
        ?? throw new EntityNotFoundException<RealmEntity>(realmId);
    }

    return (await MapAsync(realm, cancellationToken, sender)).Single();
  }

  public async Task<Sender?> ReadDefaultAsync(string? realmIdOrUniqueSlug, CancellationToken cancellationToken)
  {
    Realm? realm = null;
    if (!string.IsNullOrWhiteSpace(realmIdOrUniqueSlug))
    {
      realm = await _realmQuerier.FindAsync(realmIdOrUniqueSlug, cancellationToken)
        ?? throw new EntityNotFoundException<RealmEntity>(realmIdOrUniqueSlug);
    }

    string? tenantId = realm == null ? null : new AggregateId(realm.Id).Value;

    SenderEntity? sender = await _senders.AsNoTracking()
      .SingleOrDefaultAsync(x => x.TenantId == tenantId && x.IsDefault, cancellationToken);
    if (sender == null)
    {
      return null;
    }

    return (await MapAsync(realm, cancellationToken, sender)).Single();
  }

  public async Task<SearchResults<Sender>> SearchAsync(SearchSendersPayload payload, CancellationToken cancellationToken)
  {
    Realm? realm = null;
    string? tenantId = null;
    if (!string.IsNullOrWhiteSpace(payload.Realm))
    {
      realm = await _realmQuerier.FindAsync(payload.Realm, cancellationToken)
        ?? throw new EntityNotFoundException<RealmEntity>(payload.Realm);
      tenantId = new AggregateId(realm.Id).Value;
    }

    IQueryBuilder builder = _sqlHelper.QueryFrom(Db.Senders.Table)
      .ApplyIdInFilter(Db.Senders.AggregateId, payload.IdIn)
      .Where(Db.Senders.TenantId, tenantId == null ? Operators.IsNull() : Operators.IsEqualTo(tenantId))
      .SelectAll(Db.Senders.Table);
    _sqlHelper.ApplyTextSearch(builder, payload.Search, Db.Senders.EmailAddress, Db.Senders.DisplayName);

    if (payload.Provider.HasValue)
    {
      builder = builder.Where(Db.Senders.Provider, Operators.IsEqualTo(payload.Provider.Value.ToString()));
    }

    IQueryable<SenderEntity> query = _senders.FromQuery(builder.Build())
      .AsNoTracking();
    long total = await query.LongCountAsync(cancellationToken);

    IOrderedQueryable<SenderEntity>? ordered = null;
    foreach (SenderSortOption sort in payload.Sort)
    {
      switch (sort.Field)
      {
        case SenderSort.DisplayName:
          ordered = (ordered == null)
            ? (sort.IsDescending ? query.OrderByDescending(x => x.DisplayName) : query.OrderBy(x => x.DisplayName))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.DisplayName) : ordered.ThenBy(x => x.DisplayName));
          break;
        case SenderSort.EmailAddress:
          ordered = (ordered == null)
            ? (sort.IsDescending ? query.OrderByDescending(x => x.EmailAddress) : query.OrderBy(x => x.EmailAddress))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.EmailAddress) : ordered.ThenBy(x => x.EmailAddress));
          break;
        case SenderSort.UpdatedOn:
          ordered = (ordered == null)
            ? (sort.IsDescending ? query.OrderByDescending(x => x.UpdatedOn) : query.OrderBy(x => x.UpdatedOn))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.UpdatedOn) : ordered.ThenBy(x => x.UpdatedOn));
          break;
      }
    }
    query = ordered ?? query;

    query = query.ApplyPaging(payload);

    SenderEntity[] senders = await query.ToArrayAsync(cancellationToken);
    IEnumerable<Sender> results = await MapAsync(realm, cancellationToken, senders);

    return new SearchResults<Sender>(results, total);
  }

  private async Task<IEnumerable<Sender>> MapAsync(Realm? realm = null, CancellationToken cancellationToken = default, params SenderEntity[] senders)
  {
    IEnumerable<ActorId> actorIds = senders.SelectMany(sender => sender.GetActorIds()).Distinct();
    Dictionary<ActorId, Actor> actors = await _actorService.FindAsync(actorIds, cancellationToken);
    Mapper mapper = new(actors);

    return senders.Select(sender => mapper.ToSender(sender, realm));
  }
}
