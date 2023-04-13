using Logitar.EventSourcing;
using Logitar.EventSourcing.EntityFrameworkCore.PostgreSQL;
using Logitar.EventSourcing.EntityFrameworkCore.PostgreSQL.Entities;
using Logitar.Portal.Core.Realms;
using Logitar.Portal.Core.Templates;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Portal.EntityFrameworkCore.PostgreSQL.Repositories;

internal class TemplateRepository : EventStore, ITemplateRepository
{
  private static string AggregateType { get; } = typeof(TemplateAggregate).GetName();

  public TemplateRepository(EventContext context, IEventBus eventBus) : base(context, eventBus)
  {
  }

  public async Task<TemplateAggregate?> LoadAsync(Guid id, CancellationToken cancellationToken)
  {
    return await LoadAsync<TemplateAggregate>(new AggregateId(id), cancellationToken);
  }

  public async Task<IEnumerable<TemplateAggregate>> LoadAsync(RealmAggregate realm, CancellationToken cancellationToken)
  {
    string aggregateId = realm.Id.Value;

    EventEntity[] events = await Context.Events.FromSqlInterpolated($@"SELECT e.* FROM ""Events"" e JOIN ""Templates"" t ON t.""AggregateId"" = e.""AggregateId"" JOIN ""Realms"" r ON r.""RealmId"" = t.""RealmId"" WHERE e.""AggregateType"" = {AggregateType} AND r.""AggregateId"" = {aggregateId}")
      .AsNoTracking()
      .OrderBy(x => x.Version)
      .ToArrayAsync(cancellationToken);

    return Load<TemplateAggregate>(events);
  }

  public async Task<TemplateAggregate?> LoadByUniqueNameAsync(RealmAggregate realm, string uniqueName, CancellationToken cancellationToken)
  {
    string aggregateId = realm.Id.Value;

    EventEntity[] events = await Context.Events.FromSqlInterpolated($@"SELECT e.* FROM ""Events"" e JOIN ""Templates"" t ON t.""AggregateId"" = e.""AggregateId"" JOIN ""Realms"" r ON r.""RealmId"" = t.""RealmId"" WHERE e.""AggregateType"" = {AggregateType} AND r.""AggregateId"" = {aggregateId} AND t.""UniqueNameNormalized"" = {uniqueName.ToUpper()}")
      .AsNoTracking()
      .OrderBy(x => x.Version)
      .ToArrayAsync(cancellationToken);

    return Load<TemplateAggregate>(events).SingleOrDefault();
  }

  public async Task SaveAsync(TemplateAggregate template, CancellationToken cancellationToken)
  {
    await base.SaveAsync(template, cancellationToken);
  }

  public async Task SaveAsync(IEnumerable<TemplateAggregate> templates, CancellationToken cancellationToken)
  {
    await base.SaveAsync(templates, cancellationToken);
  }
}
