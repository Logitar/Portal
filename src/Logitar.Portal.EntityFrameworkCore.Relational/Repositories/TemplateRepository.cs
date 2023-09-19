using Logitar.Data;
using Logitar.EventSourcing;
using Logitar.EventSourcing.EntityFrameworkCore.Relational;
using Logitar.EventSourcing.Infrastructure;
using Logitar.Portal.Domain.Realms;
using Logitar.Portal.Domain.Templates;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Portal.EntityFrameworkCore.Relational.Repositories;

internal class TemplateRepository : EventSourcing.EntityFrameworkCore.Relational.AggregateRepository, ITemplateRepository
{
  private static readonly string AggregateType = typeof(TemplateAggregate).GetName();

  private readonly ISqlHelper _sqlHelper;

  public TemplateRepository(IEventBus eventBus, EventContext eventContext, IEventSerializer eventSerializer, ISqlHelper sqlHelper)
    : base(eventBus, eventContext, eventSerializer)
  {
    _sqlHelper = sqlHelper;
  }

  public async Task<TemplateAggregate?> LoadAsync(Guid id, CancellationToken cancellationToken)
    => await LoadAsync(new AggregateId(id), version: null, cancellationToken);
  public async Task<TemplateAggregate?> LoadAsync(AggregateId id, long? version, CancellationToken cancellationToken)
    => await base.LoadAsync<TemplateAggregate>(id, version, cancellationToken);

  public async Task<TemplateAggregate?> LoadAsync(string? tenantId, string uniqueName, CancellationToken cancellationToken)
  {
    tenantId = tenantId?.CleanTrim();
    string uniqueNameNormalized = uniqueName.Trim().ToUpper();

    IQuery query = _sqlHelper.QueryFrom(Db.Events.Table)
      .Join(Db.Templates.AggregateId, Db.Events.AggregateId,
        new OperatorCondition(Db.Events.AggregateType, Operators.IsEqualTo(AggregateType))
      )
      .Where(Db.Templates.TenantId, tenantId == null ? Operators.IsNull() : Operators.IsEqualTo(tenantId))
      .Where(Db.Templates.UniqueNameNormalized, Operators.IsEqualTo(uniqueNameNormalized))
      .SelectAll(Db.Events.Table)
      .Build();

    EventEntity[] events = await EventContext.Events.FromQuery(query)
      .AsNoTracking()
      .OrderBy(e => e.Version)
      .ToArrayAsync(cancellationToken);

    return base.Load<TemplateAggregate>(events.Select(EventSerializer.Deserialize)).SingleOrDefault();
  }

  public async Task<IEnumerable<TemplateAggregate>> LoadAsync(RealmAggregate? realm, CancellationToken cancellationToken)
  {
    string? tenantId = realm?.Id.Value;

    IQuery query = _sqlHelper.QueryFrom(Db.Events.Table)
      .Join(Db.Templates.AggregateId, Db.Events.AggregateId,
        new OperatorCondition(Db.Events.AggregateType, Operators.IsEqualTo(AggregateType))
      )
      .Where(Db.Templates.TenantId, tenantId == null ? Operators.IsNull() : Operators.IsEqualTo(tenantId))
      .SelectAll(Db.Events.Table)
      .Build();

    EventEntity[] events = await EventContext.Events.FromQuery(query)
      .AsNoTracking()
      .OrderBy(e => e.Version)
      .ToArrayAsync(cancellationToken);

    return base.Load<TemplateAggregate>(events.Select(EventSerializer.Deserialize));
  }

  public async Task SaveAsync(TemplateAggregate template, CancellationToken cancellationToken)
    => await base.SaveAsync(template, cancellationToken);
  public async Task SaveAsync(IEnumerable<TemplateAggregate> templates, CancellationToken cancellationToken)
    => await base.SaveAsync(templates, cancellationToken);
}
