using Logitar.Data;
using Logitar.EventSourcing;
using Logitar.Portal.Application.Actors;
using Logitar.Portal.Application.Realms;
using Logitar.Portal.Application.Users;
using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Actors;
using Logitar.Portal.Contracts.Realms;
using Logitar.Portal.Contracts.Users;
using Logitar.Portal.Domain.Users;
using Logitar.Portal.EntityFrameworkCore.Relational.Actors;
using Logitar.Portal.EntityFrameworkCore.Relational.Entities;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Portal.EntityFrameworkCore.Relational.Queriers;

internal class UserQuerier : IUserQuerier
{
  private readonly IActorService _actorService;
  private readonly IRealmQuerier _realmQuerier;
  private readonly ISqlHelper _sqlHelper;
  private readonly DbSet<UserEntity> _users;

  public UserQuerier(IActorService actorService, PortalContext context, IRealmQuerier realmQuerier, ISqlHelper sqlHelper)
  {
    _actorService = actorService;
    _realmQuerier = realmQuerier;
    _sqlHelper = sqlHelper;
    _users = context.Users;
  }

  public async Task<User> ReadAsync(UserAggregate user, CancellationToken cancellationToken)
  {
    return await ReadAsync(user.Id, cancellationToken)
      ?? throw new EntityNotFoundException<UserEntity>(user.Id);
  }
  public async Task<User?> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    return await ReadAsync(new AggregateId(id), cancellationToken);
  }
  private async Task<User?> ReadAsync(AggregateId id, CancellationToken cancellationToken)
  {
    string aggregateId = id.Value;

    UserEntity? user = await _users.AsNoTracking()
      .Include(x => x.Identifiers)
      .Include(x => x.Roles)
      .SingleOrDefaultAsync(x => x.AggregateId == aggregateId, cancellationToken);
    if (user == null)
    {
      return null;
    }

    Realm? realm = null;
    if (user.TenantId != null)
    {
      AggregateId realmId = new(user.TenantId);
      realm = await _realmQuerier.ReadAsync(realmId, cancellationToken)
        ?? throw new EntityNotFoundException<RealmEntity>(realmId);
    }

    return (await MapAsync(realm, cancellationToken, user)).Single();
  }

  public async Task<User?> ReadAsync(string? realmIdOrUniqueSlug, string uniqueName, CancellationToken cancellationToken)
  {
    Realm? realm = null;
    if (!string.IsNullOrWhiteSpace(realmIdOrUniqueSlug))
    {
      realm = await _realmQuerier.FindAsync(realmIdOrUniqueSlug, cancellationToken)
        ?? throw new EntityNotFoundException<RealmEntity>(realmIdOrUniqueSlug);
    }

    string? tenantId = realm == null ? null : new AggregateId(realm.Id).Value;
    string uniqueNameNormalized = uniqueName.Trim().ToUpper();

    UserEntity? user = await _users.AsNoTracking()
      .Include(x => x.Identifiers)
      .Include(x => x.Roles)
      .SingleOrDefaultAsync(x => x.TenantId == tenantId && x.UniqueNameNormalized == uniqueNameNormalized, cancellationToken);
    if (user == null)
    {
      return null;
    }

    return (await MapAsync(realm, cancellationToken, user)).Single();
  }

  public async Task<User?> ReadAsync(string? realmIdOrUniqueSlug, string identifierKey, string identifierValue, CancellationToken cancellationToken)
  {
    Realm? realm = null;
    if (!string.IsNullOrWhiteSpace(realmIdOrUniqueSlug))
    {
      realm = await _realmQuerier.FindAsync(realmIdOrUniqueSlug, cancellationToken)
        ?? throw new EntityNotFoundException<RealmEntity>(realmIdOrUniqueSlug);
    }

    string key = identifierKey.Trim();
    string valueNormalized = identifierValue.Trim().ToUpper();

    UserEntity? user = await _users.AsNoTracking()
      .Include(x => x.Identifiers)
      .Include(x => x.Roles)
      .SingleOrDefaultAsync(x => x.Identifiers.Any(y => y.Key == key && y.ValueNormalized == valueNormalized), cancellationToken);
    if (user == null)
    {
      return null;
    }

    return (await MapAsync(realm, cancellationToken, user)).Single();
  }

  public async Task<SearchResults<User>> SearchAsync(SearchUsersPayload payload, CancellationToken cancellationToken)
  {
    Realm? realm = null;
    string? tenantId = null;
    if (!string.IsNullOrWhiteSpace(payload.Realm))
    {
      realm = await _realmQuerier.FindAsync(payload.Realm, cancellationToken)
        ?? throw new EntityNotFoundException<RealmEntity>(payload.Realm);
      tenantId = new AggregateId(realm.Id).Value;
    }

    IQueryBuilder builder = _sqlHelper.QueryFrom(Db.Users.Table)
      .ApplyIdInFilter(Db.Users.AggregateId, payload.IdIn)
      .Where(Db.Roles.TenantId, tenantId == null ? Operators.IsNull() : Operators.IsEqualTo(tenantId))
      .SelectAll(Db.Users.Table);
    _sqlHelper.ApplyTextSearch(builder, payload.Search, Db.Users.UniqueName, Db.Users.AddressFormatted,
      Db.Users.EmailAddress, Db.Users.PhoneE164Formatted, Db.Users.FirstName, Db.Users.MiddleName,
      Db.Users.LastName, Db.Users.FullName, Db.Users.Nickname, Db.Users.Gender, Db.Users.Locale, Db.Users.TimeZone);

    if (payload.HasPassword.HasValue)
    {
      builder = builder.Where(Db.Users.HasPassword, Operators.IsEqualTo(payload.HasPassword.Value));
    }
    if (payload.IsConfirmed.HasValue)
    {
      builder = builder.Where(Db.Users.IsConfirmed, Operators.IsEqualTo(payload.IsConfirmed.Value));
    }
    if (payload.IsDisabled.HasValue)
    {
      builder = builder.Where(Db.Users.IsDisabled, Operators.IsEqualTo(payload.IsDisabled.Value));
    }

    IQueryable<UserEntity> query = _users.FromQuery(builder.Build())
      .AsNoTracking()
      .Include(x => x.Identifiers)
      .Include(x => x.Roles);
    long total = await query.LongCountAsync(cancellationToken);

    IOrderedQueryable<UserEntity>? ordered = null;
    foreach (UserSortOption sort in payload.Sort)
    {
      switch (sort.Field)
      {
        case UserSort.AuthenticatedOn:
          ordered = (ordered == null)
            ? (sort.IsDescending ? query.OrderByDescending(x => x.AuthenticatedOn) : query.OrderBy(x => x.AuthenticatedOn))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.AuthenticatedOn) : ordered.ThenBy(x => x.AuthenticatedOn));
          break;
        case UserSort.Birthdate:
          ordered = (ordered == null)
            ? (sort.IsDescending ? query.OrderByDescending(x => x.Birthdate) : query.OrderBy(x => x.Birthdate))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.Birthdate) : ordered.ThenBy(x => x.Birthdate));
          break;
        case UserSort.DisabledOn:
          ordered = (ordered == null)
            ? (sort.IsDescending ? query.OrderByDescending(x => x.DisabledOn) : query.OrderBy(x => x.DisabledOn))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.DisabledOn) : ordered.ThenBy(x => x.DisabledOn));
          break;
        case UserSort.EmailAddress:
          ordered = (ordered == null)
            ? (sort.IsDescending ? query.OrderByDescending(x => x.EmailAddress) : query.OrderBy(x => x.EmailAddress))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.EmailAddress) : ordered.ThenBy(x => x.EmailAddress));
          break;
        case UserSort.FirstName:
          ordered = (ordered == null)
            ? (sort.IsDescending ? query.OrderByDescending(x => x.FirstName) : query.OrderBy(x => x.FirstName))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.FirstName) : ordered.ThenBy(x => x.FirstName));
          break;
        case UserSort.FullName:
          ordered = (ordered == null)
            ? (sort.IsDescending ? query.OrderByDescending(x => x.FullName) : query.OrderBy(x => x.FullName))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.FullName) : ordered.ThenBy(x => x.FullName));
          break;
        case UserSort.LastName:
          ordered = (ordered == null)
            ? (sort.IsDescending ? query.OrderByDescending(x => x.LastName) : query.OrderBy(x => x.LastName))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.LastName) : ordered.ThenBy(x => x.LastName));
          break;
        case UserSort.MiddleName:
          ordered = (ordered == null)
            ? (sort.IsDescending ? query.OrderByDescending(x => x.MiddleName) : query.OrderBy(x => x.MiddleName))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.MiddleName) : ordered.ThenBy(x => x.MiddleName));
          break;
        case UserSort.Nickname:
          ordered = (ordered == null)
            ? (sort.IsDescending ? query.OrderByDescending(x => x.Nickname) : query.OrderBy(x => x.Nickname))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.Nickname) : ordered.ThenBy(x => x.Nickname));
          break;
        case UserSort.PasswordChangedOn:
          ordered = (ordered == null)
            ? (sort.IsDescending ? query.OrderByDescending(x => x.PasswordChangedOn) : query.OrderBy(x => x.PasswordChangedOn))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.PasswordChangedOn) : ordered.ThenBy(x => x.PasswordChangedOn));
          break;
        case UserSort.PhoneNumber:
          ordered = (ordered == null)
            ? (sort.IsDescending ? query.OrderByDescending(x => x.PhoneE164Formatted) : query.OrderBy(x => x.PhoneE164Formatted))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.PhoneE164Formatted) : ordered.ThenBy(x => x.PhoneE164Formatted));
          break;
        case UserSort.UniqueName:
          ordered = (ordered == null)
            ? (sort.IsDescending ? query.OrderByDescending(x => x.UniqueName) : query.OrderBy(x => x.UniqueName))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.UniqueName) : ordered.ThenBy(x => x.UniqueName));
          break;
        case UserSort.UpdatedOn:
          ordered = (ordered == null)
            ? (sort.IsDescending ? query.OrderByDescending(x => x.UpdatedOn) : query.OrderBy(x => x.UpdatedOn))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.UpdatedOn) : ordered.ThenBy(x => x.UpdatedOn));
          break;
      }
    }
    query = ordered ?? query;

    query = query.ApplyPaging(payload);

    UserEntity[] users = await query.ToArrayAsync(cancellationToken);
    IEnumerable<User> results = await MapAsync(realm, cancellationToken, users);

    return new SearchResults<User>(results, total);
  }

  private async Task<IEnumerable<User>> MapAsync(Realm? realm = null, CancellationToken cancellationToken = default, params UserEntity[] users)
  {
    IEnumerable<ActorId> actorIds = users.SelectMany(user => user.GetActorIds()).Distinct();
    Dictionary<ActorId, Actor> actors = await _actorService.FindAsync(actorIds, cancellationToken);
    Mapper mapper = new(actors);

    return users.Select(user => mapper.ToUser(user, realm));
  }
}
