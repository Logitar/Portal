using Logitar.EventSourcing;
using Logitar.EventSourcing.EntityFrameworkCore.PostgreSQL;
using Logitar.EventSourcing.EntityFrameworkCore.PostgreSQL.Entities;
using Logitar.Portal.Core.Realms;
using Logitar.Portal.Core.Senders;
using Logitar.Portal.Core.Templates;
using Logitar.Portal.Core.Users;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Portal.EntityFrameworkCore.PostgreSQL.Repositories;

internal class RealmRepository : EventStore, IRealmRepository
{
  private static string AggregateType { get; } = typeof(RealmAggregate).GetName();

  public RealmRepository(EventContext context, IEventBus eventBus) : base(context, eventBus)
  {
  }

  public async Task<RealmAggregate?> LoadAsync(Guid id, CancellationToken cancellationToken)
  {
    return await LoadAsync<RealmAggregate>(new AggregateId(id), cancellationToken);
  }

  public async Task<RealmAggregate?> LoadAsync(string idOrUniqueName, CancellationToken cancellationToken)
  {
    string aggregateId = (Guid.TryParse(idOrUniqueName, out Guid id)
      ? new AggregateId(id)
      : new(idOrUniqueName)).ToString();

    EventEntity[] events = await Context.Events.FromSqlInterpolated($@"SELECT e.* FROM ""Events"" e JOIN ""Realms"" r on r.""AggregateId"" = e.""AggregateId"" WHERE e.""AggregateType"" = {AggregateType} AND (r.""AggregateId"" = {aggregateId} OR r.""UniqueNameNormalized"" = {idOrUniqueName.ToUpper()})")
      .AsNoTracking()
      .OrderBy(x => x.Version)
      .ToArrayAsync(cancellationToken);

    return Load<RealmAggregate>(events).SingleOrDefault();
  }

  public async Task<RealmAggregate?> LoadAsync(SenderAggregate sender, CancellationToken cancellationToken)
  {
    if (!sender.RealmId.HasValue)
    {
      return null;
    }

    return await LoadAsync<RealmAggregate>(sender.RealmId.Value, cancellationToken)
      ?? throw new InvalidOperationException($"The realm '{sender.RealmId}' could not be found.");
  }

  public async Task<RealmAggregate?> LoadAsync(TemplateAggregate template, CancellationToken cancellationToken)
  {
    if (!template.RealmId.HasValue)
    {
      return null;
    }

    return await LoadAsync<RealmAggregate>(template.RealmId.Value, cancellationToken)
      ?? throw new InvalidOperationException($"The realm '{template.RealmId}' could not be found.");
  }

  public async Task<RealmAggregate?> LoadAsync(UserAggregate user, CancellationToken cancellationToken)
  {
    if (!user.RealmId.HasValue)
    {
      return null;
    }

    return await LoadAsync<RealmAggregate>(user.RealmId.Value, cancellationToken)
      ?? throw new InvalidOperationException($"The realm '{user.RealmId}' could not be found.");
  }

  public async Task<RealmAggregate?> LoadByUniqueNameAsync(string uniqueName, CancellationToken cancellationToken)
  {
    EventEntity[] events = await Context.Events.FromSqlInterpolated($@"SELECT e.* FROM ""Events"" e JOIN ""Realms"" r on r.""AggregateId"" = e.""AggregateId"" WHERE e.""AggregateType"" = {AggregateType} AND r.""UniqueNameNormalized"" = {uniqueName.ToUpper()}")
      .AsNoTracking()
      .OrderBy(x => x.Version)
      .ToArrayAsync(cancellationToken);

    return Load<RealmAggregate>(events).SingleOrDefault();
  }

  public async Task SaveAsync(RealmAggregate realm, CancellationToken cancellationToken)
  {
    await base.SaveAsync(realm, cancellationToken);
  }
}
