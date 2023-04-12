using Logitar.EventSourcing;
using Logitar.EventSourcing.EntityFrameworkCore.PostgreSQL;
using Logitar.EventSourcing.EntityFrameworkCore.PostgreSQL.Entities;
using Logitar.Portal.v2.Core.Realms;
using Logitar.Portal.v2.Core.Senders;
using Logitar.Portal.v2.Core.Templates;
using Logitar.Portal.v2.Core.Users;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Portal.v2.EntityFrameworkCore.PostgreSQL.Repositories;

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

  public async Task<RealmAggregate> LoadAsync(SenderAggregate sender, CancellationToken cancellationToken)
  {
    return await LoadAsync<RealmAggregate>(sender.RealmId, cancellationToken)
      ?? throw new InvalidOperationException($"The realm '{sender.RealmId}' could not be found.");
  }

  public async Task<RealmAggregate> LoadAsync(TemplateAggregate template, CancellationToken cancellationToken)
  {
    return await LoadAsync<RealmAggregate>(template.RealmId, cancellationToken)
      ?? throw new InvalidOperationException($"The realm '{template.RealmId}' could not be found.");
  }

  public async Task<RealmAggregate> LoadAsync(UserAggregate user, CancellationToken cancellationToken)
  {
    return await LoadAsync<RealmAggregate>(user.RealmId, cancellationToken)
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
