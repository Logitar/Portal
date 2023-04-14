﻿using Logitar.EventSourcing;
using Logitar.EventSourcing.EntityFrameworkCore.PostgreSQL;
using Logitar.EventSourcing.EntityFrameworkCore.PostgreSQL.Entities;
using Logitar.Portal.Core.Realms;
using Logitar.Portal.Core.Senders;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Portal.EntityFrameworkCore.PostgreSQL.Repositories;

internal class SenderRepository : EventStore, ISenderRepository
{
  private static string AggregateType { get; } = typeof(SenderAggregate).GetName();

  public SenderRepository(EventContext context, IEventBus eventBus) : base(context, eventBus)
  {
  }

  public async Task<SenderAggregate?> LoadAsync(Guid id, CancellationToken cancellationToken)
  {
    return await LoadAsync<SenderAggregate>(new AggregateId(id), cancellationToken);
  }

  public async Task<IEnumerable<SenderAggregate>> LoadAsync(RealmAggregate? realm, CancellationToken cancellationToken)
  {
    IQueryable<EventEntity> query;
    if (realm == null)
    {
      query = Context.Events.FromSqlInterpolated($@"SELECT e.* FROM ""Events"" e JOIN ""Senders"" s ON s.""AggregateId"" = e.""AggregateId"" WHERE e.""AggregateType"" = {AggregateType} AND s.""RealmId"" IS NULL");
    }
    else
    {
      string aggregateId = realm.Id.Value;
      query = Context.Events.FromSqlInterpolated($@"SELECT e.* FROM ""Events"" e JOIN ""Senders"" s ON s.""AggregateId"" = e.""AggregateId"" JOIN ""Realms"" r ON r.""RealmId"" = s.""RealmId"" WHERE e.""AggregateType"" = {AggregateType} AND r.""AggregateId"" = {aggregateId}");
    }

    EventEntity[] events = await query.AsNoTracking()
      .OrderBy(x => x.Version)
      .ToArrayAsync(cancellationToken);

    return Load<SenderAggregate>(events);
  }

  public async Task<SenderAggregate?> LoadDefaultAsync(RealmAggregate? realm, CancellationToken cancellationToken)
  {
    IQueryable<EventEntity> query;
    if (realm == null)
    {
      query = Context.Events.FromSqlInterpolated($@"SELECT e.* FROM ""Events"" e JOIN ""Senders"" s ON s.""AggregateId"" = e.""AggregateId"" WHERE e.""AggregateType"" = {AggregateType} AND s.""RealmId"" IS NULL AND s.""IsDefault"" = true");
    }
    else
    {
      string aggregateId = realm.Id.Value;
      query = Context.Events.FromSqlInterpolated($@"SELECT e.* FROM ""Events"" e JOIN ""Senders"" s ON s.""AggregateId"" = e.""AggregateId"" JOIN ""Realms"" r ON r.""RealmId"" = s.""RealmId"" WHERE e.""AggregateType"" = {AggregateType} AND r.""AggregateId"" = {aggregateId} AND s.""IsDefault"" = true");
    }

    EventEntity[] events = await query.AsNoTracking()
      .OrderBy(x => x.Version)
      .ToArrayAsync(cancellationToken);

    return Load<SenderAggregate>(events).SingleOrDefault();
  }

  public async Task SaveAsync(SenderAggregate sender, CancellationToken cancellationToken)
  {
    await base.SaveAsync(sender, cancellationToken);
  }

  public async Task SaveAsync(IEnumerable<SenderAggregate> senders, CancellationToken cancellationToken)
  {
    await base.SaveAsync(senders, cancellationToken);
  }
}
