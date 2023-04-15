using Logitar.EventSourcing;
using Logitar.EventSourcing.EntityFrameworkCore.PostgreSQL;
using Logitar.EventSourcing.EntityFrameworkCore.PostgreSQL.Entities;
using Logitar.Portal.Core.Caching;
using Logitar.Portal.Core.Realms;
using Logitar.Portal.Core.Users;
using Logitar.Portal.Core.Users.Contact;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Portal.EntityFrameworkCore.PostgreSQL.Repositories;

internal class UserRepository : EventStore, IUserRepository
{
  private static string AggregateType { get; } = typeof(UserAggregate).GetName();

  private readonly ICacheService _cacheService;

  public UserRepository(ICacheService cacheService,
    EventContext context,
    IEventBus eventBus) : base(context, eventBus)
  {
    _cacheService = cacheService;
  }

  public async Task<UserAggregate?> LoadAsync(Guid id, CancellationToken cancellationToken)
  {
    return await LoadAsync(new AggregateId(id), cancellationToken);
  }
  public async Task<UserAggregate?> LoadAsync(AggregateId id, CancellationToken cancellationToken)
  {
    return await LoadAsync<UserAggregate>(id, cancellationToken);
  }

  public async Task<IEnumerable<UserAggregate>> LoadAsync(RealmAggregate? realm, CancellationToken cancellationToken)
  {
    IQueryable<EventEntity> query;
    if (realm == null)
    {
      query = Context.Events.FromSqlInterpolated($@"SELECT e.* FROM ""Events"" e JOIN ""Users"" u ON u.""AggregateId"" = e.""AggregateId"" WHERE e.""AggregateType"" = {AggregateType} AND u.""RealmId"" IS NULL");
    }
    else
    {
      string aggregateId = realm.Id.Value;
      query = Context.Events.FromSqlInterpolated($@"SELECT e.* FROM ""Events"" e JOIN ""Users"" u ON u.""AggregateId"" = e.""AggregateId"" JOIN ""Realms"" r ON r.""RealmId"" = u.""RealmId"" WHERE e.""AggregateType"" = {AggregateType} AND r.""AggregateId"" = {aggregateId}");
    }

    EventEntity[] events = await query.AsNoTracking()
      .OrderBy(x => x.Version)
      .ToArrayAsync(cancellationToken);

    return Load<UserAggregate>(events);
  }

  public async Task<UserAggregate?> LoadAsync(RealmAggregate? realm, string username, CancellationToken cancellationToken)
  {
    IQueryable<EventEntity> query;
    if (realm == null)
    {
      query = Context.Events.FromSqlInterpolated($@"SELECT e.* FROM ""Events"" e JOIN ""Users"" u ON u.""AggregateId"" = e.""AggregateId"" WHERE e.""AggregateType"" = {AggregateType} AND u.""RealmId"" IS NULL AND u.""UsernameNormalized"" = {username.ToUpper()}");
    }
    else
    {
      string aggregateId = realm.Id.Value;
      query = Context.Events.FromSqlInterpolated($@"SELECT e.* FROM ""Events"" e JOIN ""Users"" u ON u.""AggregateId"" = e.""AggregateId"" JOIN ""Realms"" r ON r.""RealmId"" = u.""RealmId"" WHERE e.""AggregateType"" = {AggregateType} AND r.""AggregateId"" = {aggregateId} AND u.""UsernameNormalized"" = {username.ToUpper()}");
    }

    EventEntity[] events = await query.AsNoTracking()
      .OrderBy(x => x.Version)
      .ToArrayAsync(cancellationToken);

    return Load<UserAggregate>(events).SingleOrDefault();
  }

  public async Task<IEnumerable<UserAggregate>> LoadAsync(RealmAggregate realm, ReadOnlyEmail email, CancellationToken cancellationToken)
  {
    string? aggregateId = realm?.Id.Value;

    EventEntity[] events = await Context.Events.FromSqlInterpolated($@"SELECT e.* FROM ""Events"" e JOIN ""Users"" u ON u.""AggregateId"" = e.""AggregateId"" JOIN ""Realms"" r ON r.""RealmId"" = u.""RealmId"" WHERE e.""AggregateType"" = {AggregateType} AND r.""AggregateId"" = {aggregateId} AND u.""EmailAddressNormalized"" = {email.Address.ToUpper()}")
      .AsNoTracking()
      .OrderBy(x => x.Version)
      .ToArrayAsync(cancellationToken);

    return Load<UserAggregate>(events);
  }

  public async Task<UserAggregate?> LoadAsync(RealmAggregate? realm, string externalKey, string externalValue, CancellationToken cancellationToken)
  {
    IQueryable<EventEntity> query;
    if (realm == null)
    {
      query = Context.Events.FromSqlInterpolated($@"SELECT e.* FROM ""Events"" e JOIN ""Users"" u ON u.""AggregateId"" = e.""AggregateId"" JOIN ""ExternalIdentifiers"" x ON x.""UserId"" = u.""UserId"" WHERE e.""AggregateType"" = {AggregateType} AND u.""RealmId"" IS NULL AND x.""Key"" = {externalKey} AND x.""ValueNormalized"" = {externalValue.ToUpper()}");
    }
    else
    {
      string aggregateId = realm.Id.Value;
      query = Context.Events.FromSqlInterpolated($@"SELECT e.* FROM ""Events"" e JOIN ""Users"" u ON u.""AggregateId"" = e.""AggregateId"" JOIN ""Realms"" r ON r.""RealmId"" = u.""RealmId"" JOIN ""ExternalIdentifiers"" x ON x.""UserId"" = u.""UserId"" WHERE e.""AggregateType"" = {AggregateType} AND r.""AggregateId"" = {aggregateId} AND x.""Key"" = {externalKey} AND x.""ValueNormalized"" = {externalValue.ToUpper()}");
    }

    EventEntity[] events = await query.AsNoTracking()
      .OrderBy(x => x.Version)
      .ToArrayAsync(cancellationToken);

    return Load<UserAggregate>(events).SingleOrDefault();
  }

  public async Task SaveAsync(UserAggregate user, CancellationToken cancellationToken)
  {
    if (user.HasChanges)
    {
      _cacheService.RemoveUser(user);
    }

    await base.SaveAsync(user, cancellationToken);
  }

  public async Task SaveAsync(IEnumerable<UserAggregate> users, CancellationToken cancellationToken)
  {
    foreach (UserAggregate user in users)
    {
      if (user.HasChanges)
      {
        _cacheService.RemoveUser(user);
      }
    }

    await base.SaveAsync(users, cancellationToken);
  }
}
