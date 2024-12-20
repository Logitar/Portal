﻿using Logitar.Data;
using Logitar.EventSourcing;
using Logitar.Identity.EntityFrameworkCore.Relational;
using Logitar.Portal.Application;
using Logitar.Portal.Application.Senders;
using Logitar.Portal.Contracts.Actors;
using Logitar.Portal.Contracts.Realms;
using Logitar.Portal.Contracts.Search;
using Logitar.Portal.Contracts.Senders;
using Logitar.Portal.Domain.Senders;
using Logitar.Portal.EntityFrameworkCore.Relational.Actors;
using Logitar.Portal.EntityFrameworkCore.Relational.Entities;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Portal.EntityFrameworkCore.Relational.Queriers;

internal class SenderQuerier : ISenderQuerier
{
  private readonly IActorService _actorService;
  private readonly ISearchHelper _searchHelper;
  private readonly DbSet<SenderEntity> _senders;
  private readonly ISqlHelper _sqlHelper;

  public SenderQuerier(IActorService actorService, PortalContext context, ISearchHelper searchHelper, ISqlHelper sqlHelper)
  {
    _actorService = actorService;
    _searchHelper = searchHelper;
    _senders = context.Senders;
    _sqlHelper = sqlHelper;
  }

  public async Task<Sender> ReadAsync(Realm? realm, SenderAggregate sender, CancellationToken cancellationToken)
  {
    return await ReadAsync(realm, sender.Id, cancellationToken)
      ?? throw new InvalidOperationException($"The sender entity 'AggregateId={sender.Id.Value}' could not be found.");
  }
  public async Task<Sender?> ReadAsync(Realm? realm, SenderId id, CancellationToken cancellationToken)
    => await ReadAsync(realm, id.ToGuid(), cancellationToken);
  public async Task<Sender?> ReadAsync(Realm? realm, Guid id, CancellationToken cancellationToken)
  {
    string aggregateId = new AggregateId(id).Value;

    SenderEntity? sender = await _senders.AsNoTracking()
      .SingleOrDefaultAsync(x => x.AggregateId == aggregateId, cancellationToken);

    if (sender == null || sender.TenantId != realm?.GetTenantId().Value)
    {
      return null;
    }

    return await MapAsync(sender, realm, cancellationToken);
  }

  public async Task<Sender?> ReadDefaultAsync(Realm? realm, CancellationToken cancellationToken)
  {
    string? tenantId = realm?.GetTenantId().Value;

    SenderEntity? sender = await _senders.AsNoTracking()
      .SingleOrDefaultAsync(x => x.TenantId == tenantId && x.IsDefault, cancellationToken);

    if (sender == null)
    {
      return null;
    }

    return await MapAsync(sender, realm, cancellationToken);
  }

  public async Task<SearchResults<Sender>> SearchAsync(Realm? realm, SearchSendersPayload payload, CancellationToken cancellationToken)
  {
    IQueryBuilder builder = _sqlHelper.QueryFrom(PortalDb.Senders.Table).SelectAll(PortalDb.Senders.Table)
      .ApplyRealmFilter(PortalDb.Senders.TenantId, realm)
      .ApplyIdFilter(PortalDb.Senders.AggregateId, payload.Ids);
    _searchHelper.ApplyTextSearch(builder, payload.Search, PortalDb.Senders.EmailAddress, PortalDb.Senders.DisplayName, PortalDb.Senders.DisplayName);

    if (payload.Provider.HasValue)
    {
      builder.Where(PortalDb.Senders.Provider, Operators.IsEqualTo(payload.Provider.Value.ToString()));
    }

    IQueryable<SenderEntity> query = _senders.FromQuery(builder).AsNoTracking();

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
        case SenderSort.PhoneNumber:
          ordered = (ordered == null)
            ? (sort.IsDescending ? query.OrderByDescending(x => x.PhoneNumber) : query.OrderBy(x => x.PhoneNumber))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.PhoneNumber) : ordered.ThenBy(x => x.PhoneNumber));
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
    IEnumerable<Sender> items = await MapAsync(senders, realm, cancellationToken);

    return new SearchResults<Sender>(items, total);
  }

  private async Task<Sender> MapAsync(SenderEntity sender, Realm? realm, CancellationToken cancellationToken = default)
    => (await MapAsync([sender], realm, cancellationToken)).Single();
  private async Task<IEnumerable<Sender>> MapAsync(IEnumerable<SenderEntity> senders, Realm? realm, CancellationToken cancellationToken = default)
  {
    IEnumerable<ActorId> actorIds = senders.SelectMany(sender => sender.GetActorIds());
    IEnumerable<Actor> actors = await _actorService.FindAsync(actorIds, cancellationToken);
    Mapper mapper = new(actors);

    return senders.Select(sender => mapper.ToSender(sender, realm));
  }
}
