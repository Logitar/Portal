using Logitar.Data;
using Logitar.EventSourcing;
using Logitar.EventSourcing.EntityFrameworkCore.Relational;
using Logitar.EventSourcing.Infrastructure;
using Logitar.Identity.EntityFrameworkCore.Relational;
using Logitar.Portal.Domain.Templates;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Portal.EntityFrameworkCore.Relational.Repositories;

internal class TemplateRepository : EventSourcing.EntityFrameworkCore.Relational.AggregateRepository, ITemplateRepository
{
  private static readonly string AggregateType = typeof(TemplateAggregate).GetNamespaceQualifiedName();

  private readonly ISqlHelper _sqlHelper;

  public TemplateRepository(IEventBus eventBus, EventContext eventContext, IEventSerializer eventSerializer, ISqlHelper sqlHelper)
    : base(eventBus, eventContext, eventSerializer)
  {
    _sqlHelper = sqlHelper;
  }

  public async Task<IEnumerable<TemplateAggregate>> LoadAsync(CancellationToken cancellationToken)
    => await LoadAsync<TemplateAggregate>(cancellationToken);

  public async Task<TemplateAggregate?> LoadAsync(Guid id, CancellationToken cancellationToken)
    => await LoadAsync<TemplateAggregate>(new AggregateId(id), cancellationToken);

  public async Task<TemplateAggregate?> LoadAsync(TemplateId id, CancellationToken cancellationToken)
    => await LoadAsync(id, version: null, cancellationToken);
  public async Task<TemplateAggregate?> LoadAsync(TemplateId id, long? version, CancellationToken cancellationToken)
    => await LoadAsync<TemplateAggregate>(id.AggregateId, version, cancellationToken);

  public async Task<TemplateAggregate?> LoadAsync(UniqueKeyUnit uniqueKey, CancellationToken cancellationToken)
  {
    IQuery query = _sqlHelper.QueryFrom(EventDb.Events.Table)
      .Join(PortalDb.Templates.AggregateId, EventDb.Events.AggregateId,
        new OperatorCondition(EventDb.Events.AggregateType, Operators.IsEqualTo(AggregateType))
      )
      .Where(PortalDb.Templates.UniqueKeyNormalized, Operators.IsEqualTo(uniqueKey.Value.ToUpper()))
      .SelectAll(EventDb.Events.Table)
      .Build();

    EventEntity[] events = await EventContext.Events.FromQuery(query)
      .AsNoTracking()
      .OrderBy(e => e.Version)
      .ToArrayAsync(cancellationToken);

    return Load<TemplateAggregate>(events.Select(EventSerializer.Deserialize)).SingleOrDefault();
  }

  public async Task SaveAsync(TemplateAggregate template, CancellationToken cancellationToken)
    => await base.SaveAsync(template, cancellationToken);
  public async Task SaveAsync(IEnumerable<TemplateAggregate> templates, CancellationToken cancellationToken)
    => await base.SaveAsync(templates, cancellationToken);
}
