using Logitar.Data;
using Logitar.EventSourcing;
using Logitar.EventSourcing.EntityFrameworkCore.Relational;
using Logitar.EventSourcing.Infrastructure;
using Logitar.Portal.Domain.Realms;
using Logitar.Portal.Domain.Roles;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Portal.EntityFrameworkCore.Relational.Repositories;

internal class RoleRepository : EventSourcing.EntityFrameworkCore.Relational.AggregateRepository, IRoleRepository
{
  private static readonly string AggregateType = typeof(RoleAggregate).GetName();

  private readonly ISqlHelper _sqlHelper;

  public RoleRepository(IEventBus eventBus, EventContext eventContext, IEventSerializer eventSerializer, ISqlHelper sqlHelper)
    : base(eventBus, eventContext, eventSerializer)
  {
    _sqlHelper = sqlHelper;
  }

  public async Task<RoleAggregate?> LoadAsync(Guid id, CancellationToken cancellationToken)
    => await LoadAsync(new AggregateId(id), version: null, cancellationToken);
  public async Task<RoleAggregate?> LoadAsync(AggregateId id, long? version, CancellationToken cancellationToken)
    => await base.LoadAsync<RoleAggregate>(id, version, cancellationToken);

  public async Task<RoleAggregate?> LoadAsync(string? tenantId, string uniqueName, CancellationToken cancellationToken)
  {
    tenantId = tenantId?.CleanTrim();
    string uniqueNameNormalized = uniqueName.Trim().ToUpper();

    IQuery query = _sqlHelper.QueryFrom(Db.Events.Table)
      .Join(Db.Roles.AggregateId, Db.Events.AggregateId,
        new OperatorCondition(Db.Events.AggregateType, Operators.IsEqualTo(AggregateType))
      )
      .Where(Db.Roles.TenantId, tenantId == null ? Operators.IsNull() : Operators.IsEqualTo(tenantId))
      .Where(Db.Roles.UniqueNameNormalized, Operators.IsEqualTo(uniqueNameNormalized))
      .SelectAll(Db.Events.Table)
      .Build();

    EventEntity[] events = await EventContext.Events.FromQuery(query)
      .AsNoTracking()
      .OrderBy(e => e.Version)
      .ToArrayAsync(cancellationToken);

    return base.Load<RoleAggregate>(events.Select(EventSerializer.Deserialize)).SingleOrDefault();
  }

  public async Task<IEnumerable<RoleAggregate>> LoadAsync(RealmAggregate? realm, CancellationToken cancellationToken)
  {
    string? tenantId = realm?.Id.Value;

    IQuery query = _sqlHelper.QueryFrom(Db.Events.Table)
      .Join(Db.Roles.AggregateId, Db.Events.AggregateId,
        new OperatorCondition(Db.Events.AggregateType, Operators.IsEqualTo(AggregateType))
      )
      .Where(Db.Roles.TenantId, tenantId == null ? Operators.IsNull() : Operators.IsEqualTo(tenantId))
      .SelectAll(Db.Events.Table)
      .Build();

    EventEntity[] events = await EventContext.Events.FromQuery(query)
      .AsNoTracking()
      .OrderBy(e => e.Version)
      .ToArrayAsync(cancellationToken);

    return base.Load<RoleAggregate>(events.Select(EventSerializer.Deserialize));
  }

  public async Task SaveAsync(RoleAggregate role, CancellationToken cancellationToken)
    => await base.SaveAsync(role, cancellationToken);
  public async Task SaveAsync(IEnumerable<RoleAggregate> roles, CancellationToken cancellationToken)
    => await base.SaveAsync(roles, cancellationToken);
}
