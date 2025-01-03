﻿using Logitar.Data;
using Logitar.EventSourcing;
using Logitar.Identity.Core;
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
  private readonly IQueryHelper _queryHelper;
  private readonly DbSet<SenderEntity> _senders;

  public SenderQuerier(IActorService actorService, PortalContext context, IQueryHelper queryHelper)
  {
    _actorService = actorService;
    _queryHelper = queryHelper;
    _senders = context.Senders;
  }

  public async Task<SenderModel> ReadAsync(RealmModel? realm, Sender sender, CancellationToken cancellationToken)
  {
    return await ReadAsync(realm, sender.Id, cancellationToken)
      ?? throw new InvalidOperationException($"The sender entity 'StreamId={sender.Id.Value}' could not be found.");
  }
  public async Task<SenderModel?> ReadAsync(RealmModel? realm, Guid id, CancellationToken cancellationToken)
  {
    return await ReadAsync(realm, new SenderId(realm?.GetTenantId(), new EntityId(id)), cancellationToken);
  }
  public async Task<SenderModel?> ReadAsync(RealmModel? realm, SenderId id, CancellationToken cancellationToken)
  {
    string streamId = id.Value;

    SenderEntity? sender = await _senders.AsNoTracking()
      .SingleOrDefaultAsync(x => x.StreamId == streamId, cancellationToken);

    if (sender == null || sender.TenantId != realm?.GetTenantId().Value)
    {
      return null;
    }

    return await MapAsync(sender, realm, cancellationToken);
  }

  public async Task<SenderModel?> ReadDefaultAsync(RealmModel? realm, CancellationToken cancellationToken)
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

  public async Task<SearchResults<SenderModel>> SearchAsync(RealmModel? realm, SearchSendersPayload payload, CancellationToken cancellationToken)
  {
    IQueryBuilder builder = _queryHelper.From(PortalDb.Senders.Table).SelectAll(PortalDb.Senders.Table)
      .ApplyRealmFilter(PortalDb.Senders.TenantId, realm)
      .ApplyIdFilter(PortalDb.Senders.EntityId, payload.Ids);
    _queryHelper.ApplyTextSearch(builder, payload.Search, PortalDb.Senders.EmailAddress, PortalDb.Senders.DisplayName, PortalDb.Senders.DisplayName);

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
    IReadOnlyCollection<SenderModel> items = await MapAsync(senders, realm, cancellationToken);

    return new SearchResults<SenderModel>(items, total);
  }

  private async Task<SenderModel> MapAsync(SenderEntity sender, RealmModel? realm, CancellationToken cancellationToken = default)
    => (await MapAsync([sender], realm, cancellationToken)).Single();
  private async Task<IReadOnlyCollection<SenderModel>> MapAsync(IEnumerable<SenderEntity> senders, RealmModel? realm, CancellationToken cancellationToken = default)
  {
    IReadOnlyCollection<ActorId> actorIds = senders.SelectMany(sender => sender.GetActorIds()).ToArray();
    IReadOnlyCollection<ActorModel> actors = await _actorService.FindAsync(actorIds, cancellationToken);
    Mapper mapper = new(actors);

    return senders.Select(sender => mapper.ToSender(sender, realm)).ToArray();
  }
}
