using Logitar.EventSourcing;
using Logitar.EventSourcing.EntityFrameworkCore.PostgreSQL;
using Logitar.EventSourcing.EntityFrameworkCore.PostgreSQL.Entities;
using Logitar.Portal.v2.Core.Realms;
using Logitar.Portal.v2.Core.Users;
using Logitar.Portal.v2.Core.Users.Contact;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Portal.v2.EntityFrameworkCore.PostgreSQL.Repositories;

internal class UserRepository : EventStore, IUserRepository
{
  private static string AggregateType { get; } = typeof(UserAggregate).GetName();

  public UserRepository(EventContext context, IEventBus eventBus) : base(context, eventBus)
  {
  }

  public async Task<UserAggregate?> LoadAsync(Guid id, CancellationToken cancellationToken)
  {
    return await LoadAsync(new AggregateId(id), cancellationToken);
  }
  public async Task<UserAggregate?> LoadAsync(AggregateId id, CancellationToken cancellationToken)
  {
    return await LoadAsync<UserAggregate>(id, cancellationToken);
  }

  public async Task<IEnumerable<UserAggregate>> LoadAsync(RealmAggregate realm, CancellationToken cancellationToken)
  {
    string? aggregateId = realm?.Id.Value;

    IQueryable<EventEntity> query = Context.Events.FromSqlInterpolated($@"SELECT e.* FROM ""Events"" e JOIN ""Users"" u ON u.""AggregateId"" = e.""AggregateId"" JOIN ""Realms"" r ON r.""RealmId"" = u.""RealmId"" WHERE e.""AggregateType"" = {AggregateType} AND r.""AggregateId"" = {aggregateId}");

    EventEntity[] events = await query.AsNoTracking()
      .OrderBy(x => x.Version)
      .ToArrayAsync(cancellationToken);

    return Load<UserAggregate>(events);
  }

  public async Task<UserAggregate?> LoadAsync(RealmAggregate realm, string username, CancellationToken cancellationToken)
  {
    string? aggregateId = realm?.Id.Value;

    EventEntity[] events = await Context.Events.FromSqlInterpolated($@"SELECT e.* FROM ""Events"" e JOIN ""Users"" u ON u.""AggregateId"" = e.""AggregateId"" JOIN ""Realms"" r ON r.""RealmId"" = u.""RealmId"" WHERE e.""AggregateType"" = {AggregateType} AND r.""AggregateId"" = {aggregateId} AND u.""UsernameNormalized"" = {username.ToUpper()}")
      .AsNoTracking()
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

  public async Task<UserAggregate?> LoadAsync(RealmAggregate realm, string externalKey, string externalValue, CancellationToken cancellationToken)
  {
    string aggregateId = realm.Id.Value;

    EventEntity[] events = await Context.Events.FromSqlInterpolated($@"SELECT e.* FROM ""Events"" e JOIN ""Users"" u ON u.""AggregateId"" = e.""AggregateId"" JOIN ""Realms"" r ON r.""RealmId"" = u.""RealmId"" JOIN ""ExternalIdentifiers"" x ON x.""UserId"" = u.""UserId"" WHERE e.""AggregateType"" = {AggregateType} AND r.""AggregateId"" = {aggregateId} AND x.""Key"" = {externalKey} AND x.""ValueNormalized"" = {externalValue.ToUpper()}")
      .AsNoTracking()
      .OrderBy(x => x.Version)
      .ToArrayAsync(cancellationToken);

    return Load<UserAggregate>(events).SingleOrDefault();
  }

  public async Task SaveAsync(UserAggregate user, CancellationToken cancellationToken)
  {
    await base.SaveAsync(user, cancellationToken);
  }

  public async Task SaveAsync(IEnumerable<UserAggregate> users, CancellationToken cancellationToken)
  {
    await base.SaveAsync(users, cancellationToken);
  }
}
