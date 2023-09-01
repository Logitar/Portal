using Logitar.Data;
using Logitar.EventSourcing;
using Logitar.EventSourcing.EntityFrameworkCore.Relational;
using Logitar.EventSourcing.Infrastructure;
using Logitar.Portal.Domain.ApiKeys;
using Logitar.Portal.Domain.Realms;
using Logitar.Portal.Domain.Roles;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Portal.EntityFrameworkCore.Relational.Repositories;

internal class ApiKeyRepository : EventSourcing.EntityFrameworkCore.Relational.AggregateRepository, IApiKeyRepository
{
  private static readonly string AggregateType = typeof(ApiKeyAggregate).GetName();

  private readonly ISqlHelper _sqlHelper;

  public ApiKeyRepository(IEventBus eventBus, EventContext eventContext, IEventSerializer eventSerializer, ISqlHelper sqlHelper)
    : base(eventBus, eventContext, eventSerializer)
  {
    _sqlHelper = sqlHelper;
  }

  public async Task<ApiKeyAggregate?> LoadAsync(Guid id, CancellationToken cancellationToken)
    => await LoadAsync(new AggregateId(id), cancellationToken);
  public async Task<ApiKeyAggregate?> LoadAsync(AggregateId id, CancellationToken cancellationToken)
    => await base.LoadAsync<ApiKeyAggregate>(id, cancellationToken);

  public async Task<IEnumerable<ApiKeyAggregate>> LoadAsync(RealmAggregate? realm, CancellationToken cancellationToken)
  {
    string? tenantId = realm?.Id.Value;

    IQuery query = _sqlHelper.QueryFrom(Db.Events.Table)
      .Join(Db.ApiKeys.AggregateId, Db.Events.AggregateId,
        new OperatorCondition(Db.Events.AggregateType, Operators.IsEqualTo(AggregateType))
      )
      .Where(Db.ApiKeys.TenantId, tenantId == null ? Operators.IsNull() : Operators.IsEqualTo(tenantId))
      .SelectAll(Db.Events.Table)
      .Build();

    EventEntity[] events = await EventContext.Events.FromQuery(query)
      .AsNoTracking()
      .OrderBy(e => e.Version)
      .ToArrayAsync(cancellationToken);

    return base.Load<ApiKeyAggregate>(events.Select(EventSerializer.Deserialize));
  }

  public async Task<IEnumerable<ApiKeyAggregate>> LoadAsync(RoleAggregate role, CancellationToken cancellationToken)
  {
    string aggregateId = role.Id.Value;

    IQuery query = _sqlHelper.QueryFrom(Db.Events.Table)
      .Join(Db.ApiKeys.AggregateId, Db.Events.AggregateId,
        new OperatorCondition(Db.Events.AggregateType, Operators.IsEqualTo(AggregateType))
      )
      .Join(Db.ApiKeyRoles.ApiKeyId, Db.ApiKeys.ApiKeyId)
      .Join(Db.Roles.RoleId, Db.ApiKeyRoles.RoleId)
      .Where(Db.Roles.AggregateId, Operators.IsEqualTo(aggregateId))
      .SelectAll(Db.Events.Table)
      .Build();

    EventEntity[] events = await EventContext.Events.FromQuery(query)
      .AsNoTracking()
      .OrderBy(e => e.Version)
      .ToArrayAsync(cancellationToken);

    return base.Load<ApiKeyAggregate>(events.Select(EventSerializer.Deserialize));
  }

  public async Task SaveAsync(ApiKeyAggregate apiKey, CancellationToken cancellationToken)
    => await base.SaveAsync(apiKey, cancellationToken);
  public async Task SaveAsync(IEnumerable<ApiKeyAggregate> apiKeys, CancellationToken cancellationToken)
    => await base.SaveAsync(apiKeys, cancellationToken);
}
