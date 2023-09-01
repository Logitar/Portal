using Logitar.Data;
using Logitar.EventSourcing;
using Logitar.EventSourcing.EntityFrameworkCore.Relational;
using Logitar.EventSourcing.Infrastructure;
using Logitar.Portal.Application.Caching;
using Logitar.Portal.Contracts.Users;
using Logitar.Portal.Domain.Realms;
using Logitar.Portal.Domain.Roles;
using Logitar.Portal.Domain.Users;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Portal.EntityFrameworkCore.Relational.Repositories;

internal class UserRepository : EventSourcing.EntityFrameworkCore.Relational.AggregateRepository, IUserRepository
{
  private static readonly string AggregateType = typeof(UserAggregate).GetName();

  private readonly ICacheService _cacheService;
  private readonly ISqlHelper _sqlHelper;

  public UserRepository(ICacheService cacheService, IEventBus eventBus, EventContext eventContext, IEventSerializer eventSerializer, ISqlHelper sqlHelper)
    : base(eventBus, eventContext, eventSerializer)
  {
    _cacheService = cacheService;
    _sqlHelper = sqlHelper;
  }

  public async Task<UserAggregate?> LoadAsync(Guid id, CancellationToken cancellationToken)
    => await base.LoadAsync<UserAggregate>(new AggregateId(id), cancellationToken);
  public async Task<UserAggregate?> LoadAsync(AggregateId id, long? version, CancellationToken cancellationToken)
    => await base.LoadAsync<UserAggregate>(id, version, cancellationToken);

  public async Task<UserAggregate?> LoadAsync(string? tenantId, string uniqueName, CancellationToken cancellationToken)
  {
    tenantId = tenantId?.CleanTrim();
    string uniqueNameNormalized = uniqueName.Trim().ToUpper();

    IQuery query = _sqlHelper.QueryFrom(Db.Events.Table)
      .Join(Db.Users.AggregateId, Db.Events.AggregateId,
        new OperatorCondition(Db.Events.AggregateType, Operators.IsEqualTo(AggregateType))
      )
      .Where(Db.Users.TenantId, tenantId == null ? Operators.IsNull() : Operators.IsEqualTo(tenantId))
      .Where(Db.Users.UniqueNameNormalized, Operators.IsEqualTo(uniqueNameNormalized))
      .SelectAll(Db.Events.Table)
      .Build();

    EventEntity[] events = await EventContext.Events.FromQuery(query)
      .AsNoTracking()
      .OrderBy(e => e.Version)
      .ToArrayAsync(cancellationToken);

    return base.Load<UserAggregate>(events.Select(EventSerializer.Deserialize)).SingleOrDefault();
  }

  public async Task<UserAggregate?> LoadAsync(string? tenantId, string identifierKey, string identifierValue, CancellationToken cancellationToken)
  {
    tenantId = tenantId?.CleanTrim();
    string key = identifierKey.Trim();
    string valueNormalized = identifierValue.Trim().ToUpper();

    IQuery query = _sqlHelper.QueryFrom(Db.Events.Table)
      .Join(Db.Users.AggregateId, Db.Events.AggregateId,
        new OperatorCondition(Db.Events.AggregateType, Operators.IsEqualTo(AggregateType))
      )
      .Join(Db.UserIdentifiers.UserId, Db.Users.UserId)
      .Where(Db.Users.TenantId, tenantId == null ? Operators.IsNull() : Operators.IsEqualTo(tenantId))
      .Where(Db.UserIdentifiers.Key, Operators.IsEqualTo(key))
      .Where(Db.UserIdentifiers.ValueNormalized, Operators.IsEqualTo(valueNormalized))
      .SelectAll(Db.Events.Table)
      .Build();

    EventEntity[] events = await EventContext.Events.FromQuery(query)
      .AsNoTracking()
      .OrderBy(e => e.Version)
      .ToArrayAsync(cancellationToken);

    return base.Load<UserAggregate>(events.Select(EventSerializer.Deserialize)).SingleOrDefault();
  }

  public async Task<IEnumerable<UserAggregate>> LoadAsync(RoleAggregate role, CancellationToken cancellationToken)
  {
    string aggregateId = role.Id.Value;

    IQuery query = _sqlHelper.QueryFrom(Db.Events.Table)
      .Join(Db.Users.AggregateId, Db.Events.AggregateId,
        new OperatorCondition(Db.Events.AggregateType, Operators.IsEqualTo(AggregateType))
      )
      .Join(Db.UserRoles.UserId, Db.Users.UserId)
      .Join(Db.Roles.RoleId, Db.UserRoles.RoleId)
      .Where(Db.Roles.AggregateId, Operators.IsEqualTo(aggregateId))
      .SelectAll(Db.Events.Table)
      .Build();

    EventEntity[] events = await EventContext.Events.FromQuery(query)
      .AsNoTracking()
      .OrderBy(e => e.Version)
      .ToArrayAsync(cancellationToken);

    return base.Load<UserAggregate>(events.Select(EventSerializer.Deserialize));
  }

  public async Task<IEnumerable<UserAggregate>> LoadAsync(RealmAggregate? realm, CancellationToken cancellationToken)
  {
    string? tenantId = realm?.Id.Value;

    IQuery query = _sqlHelper.QueryFrom(Db.Events.Table)
      .Join(Db.Users.AggregateId, Db.Events.AggregateId,
        new OperatorCondition(Db.Events.AggregateType, Operators.IsEqualTo(AggregateType))
      )
      .Where(Db.Users.TenantId, tenantId == null ? Operators.IsNull() : Operators.IsEqualTo(tenantId))
      .SelectAll(Db.Events.Table)
      .Build();

    EventEntity[] events = await EventContext.Events.FromQuery(query)
      .AsNoTracking()
      .OrderBy(e => e.Version)
      .ToArrayAsync(cancellationToken);

    return base.Load<UserAggregate>(events.Select(EventSerializer.Deserialize));
  }

  public async Task<IEnumerable<UserAggregate>> LoadAsync(string? tenantId, IEmailAddress email, CancellationToken cancellationToken)
  {
    tenantId = tenantId?.CleanTrim();
    string emailAddressNormalized = email.Address.Trim().ToUpper();

    IQuery query = _sqlHelper.QueryFrom(Db.Events.Table)
      .Join(Db.Users.AggregateId, Db.Events.AggregateId,
        new OperatorCondition(Db.Events.AggregateType, Operators.IsEqualTo(AggregateType))
      )
      .Where(Db.Users.TenantId, tenantId == null ? Operators.IsNull() : Operators.IsEqualTo(tenantId))
      .Where(Db.Users.EmailAddressNormalized, Operators.IsEqualTo(emailAddressNormalized))
      .SelectAll(Db.Events.Table)
      .Build();

    EventEntity[] events = await EventContext.Events.FromQuery(query)
      .AsNoTracking()
      .OrderBy(e => e.Version)
      .ToArrayAsync(cancellationToken);

    return base.Load<UserAggregate>(events.Select(EventSerializer.Deserialize));
  }

  public async Task SaveAsync(UserAggregate user, CancellationToken cancellationToken)
  {
    await base.SaveAsync(user, cancellationToken);

    _cacheService.RemoveUser(user);
  }
  public async Task SaveAsync(IEnumerable<UserAggregate> users, CancellationToken cancellationToken)
  {
    await base.SaveAsync(users, cancellationToken);

    foreach (UserAggregate user in users)
    {
      _cacheService.RemoveUser(user);
    }
  }
}
