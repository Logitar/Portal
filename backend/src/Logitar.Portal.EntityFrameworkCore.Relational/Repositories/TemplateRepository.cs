using Logitar.Data;
using Logitar.EventSourcing;
using Logitar.EventSourcing.EntityFrameworkCore.Relational;
using Logitar.EventSourcing.Infrastructure;
using Logitar.Identity.Domain.Shared;
using Logitar.Identity.EntityFrameworkCore.Relational;
using Logitar.Portal.Domain.Templates;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Portal.EntityFrameworkCore.Relational.Repositories;

internal class TemplateRepository : EventSourcing.EntityFrameworkCore.Relational.AggregateRepository, ITemplateRepository
{
  private static readonly string AggregateType = typeof(Template).GetNamespaceQualifiedName();

  private readonly ISqlHelper _sqlHelper;

  public TemplateRepository(IEventBus eventBus, EventContext eventContext, IEventSerializer eventSerializer, ISqlHelper sqlHelper)
    : base(eventBus, eventContext, eventSerializer)
  {
    _sqlHelper = sqlHelper;
  }

  public async Task<Template?> LoadAsync(Guid id, CancellationToken cancellationToken)
    => await LoadAsync<Template>(new AggregateId(id), cancellationToken);

  public async Task<Template?> LoadAsync(TemplateId id, CancellationToken cancellationToken)
    => await LoadAsync(id, version: null, cancellationToken);
  public async Task<Template?> LoadAsync(TemplateId id, long? version, CancellationToken cancellationToken)
    => await LoadAsync<Template>(id.AggregateId, version, cancellationToken);

  public async Task<IEnumerable<Template>> LoadAsync(CancellationToken cancellationToken)
    => await LoadAsync<Template>(cancellationToken);

  public async Task<IEnumerable<Template>> LoadAsync(TenantId? tenantId, CancellationToken cancellationToken)
  {
    IQuery query = _sqlHelper.QueryFrom(EventDb.Events.Table)
      .Join(PortalDb.Templates.AggregateId, EventDb.Events.AggregateId,
        new OperatorCondition(EventDb.Events.AggregateType, Operators.IsEqualTo(AggregateType))
      )
      .Where(PortalDb.Templates.TenantId, tenantId == null ? Operators.IsNull() : Operators.IsEqualTo(tenantId.Value))
      .SelectAll(EventDb.Events.Table)
      .Build();

    EventEntity[] events = await EventContext.Events.FromQuery(query)
      .AsNoTracking()
      .OrderBy(e => e.Version)
      .ToArrayAsync(cancellationToken);

    return Load<Template>(events.Select(EventSerializer.Deserialize));
  }

  public async Task<Template?> LoadAsync(TenantId? tenantId, UniqueKeyUnit uniqueKey, CancellationToken cancellationToken)
  {
    IQuery query = _sqlHelper.QueryFrom(EventDb.Events.Table)
      .Join(PortalDb.Templates.AggregateId, EventDb.Events.AggregateId,
        new OperatorCondition(EventDb.Events.AggregateType, Operators.IsEqualTo(AggregateType))
      )
      .Where(PortalDb.Templates.TenantId, tenantId == null ? Operators.IsNull() : Operators.IsEqualTo(tenantId.Value))
      .Where(PortalDb.Templates.UniqueKeyNormalized, Operators.IsEqualTo(uniqueKey.Value.ToUpper()))
      .SelectAll(EventDb.Events.Table)
      .Build();

    EventEntity[] events = await EventContext.Events.FromQuery(query)
      .AsNoTracking()
      .OrderBy(e => e.Version)
      .ToArrayAsync(cancellationToken);

    return Load<Template>(events.Select(EventSerializer.Deserialize)).SingleOrDefault();
  }

  public async Task SaveAsync(Template template, CancellationToken cancellationToken)
    => await base.SaveAsync(template, cancellationToken);
  public async Task SaveAsync(IEnumerable<Template> templates, CancellationToken cancellationToken)
    => await base.SaveAsync(templates, cancellationToken);
}
