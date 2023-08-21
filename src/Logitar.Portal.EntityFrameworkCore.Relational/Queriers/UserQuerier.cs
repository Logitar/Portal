using Logitar.Data;
using Logitar.EventSourcing;
using Logitar.Identity.Domain.Users;
using Logitar.Identity.EntityFrameworkCore.Relational;
using Logitar.Identity.EntityFrameworkCore.Relational.Entities;
using Logitar.Portal.Application.Actors;
using Logitar.Portal.Application.Users;
using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Actors;
using Logitar.Portal.Contracts.Realms;
using Logitar.Portal.Contracts.Users;
using Logitar.Portal.EntityFrameworkCore.Relational.Actors;
using Logitar.Portal.EntityFrameworkCore.Relational.Entities;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Portal.EntityFrameworkCore.Relational.Queriers;

internal class UserQuerier : IUserQuerier
{
  private readonly IActorService _actorService;
  private readonly IQueryHelper _queryHelper;
  private readonly DbSet<RealmEntity> _realms;
  private readonly DbSet<UserEntity> _users;

  public UserQuerier(IActorService actorService, IdentityContext identityContext,
    IQueryHelper queryHelper, PortalContext portalContext)
  {
    _actorService = actorService;
    _queryHelper = queryHelper;
    _realms = portalContext.Realms;
    _users = identityContext.Users;
  }

  public async Task<User> ReadAsync(UserAggregate user, CancellationToken cancellationToken)
    => await ReadAsync(user.Id.Value, cancellationToken)
      ?? throw new InvalidOperationException($"The user entity 'Id={user.Id}' could not be found.");
  public async Task<User?> ReadAsync(string id, CancellationToken cancellationToken)
  {
    UserEntity? user = await _users.AsNoTracking()
      .Include(x => x.Roles)
      .SingleOrDefaultAsync(x => x.AggregateId == id, cancellationToken);

    RealmEntity? realm = null;
    if (user?.TenantId != null)
    {
      realm = await _realms.AsNoTracking()
        .SingleOrDefaultAsync(x => x.AggregateId == user.TenantId, cancellationToken)
        ?? throw new InvalidOperationException($"The realm 'Id={user.TenantId}' could not be found.");
    }

    return await MapAsync(user, realm, cancellationToken);
  }

  public async Task<User?> ReadAsync(string? realmIdOrUniqueSlug, string uniqueName, CancellationToken cancellationToken)
  {
    RealmEntity? realm = null;
    if (realmIdOrUniqueSlug != null)
    {
      realm = await FindRealmAsync(realmIdOrUniqueSlug, cancellationToken);
      if (realm == null)
      {
        return null;
      }
    }

    string? tenantId = realm?.AggregateId;
    string uniqueNameNormalized = uniqueName.Trim().ToUpper();

    UserEntity? user = await _users.AsNoTracking()
      .Include(x => x.Roles)
      .SingleOrDefaultAsync(x => x.TenantId == tenantId && x.UniqueNameNormalized == uniqueNameNormalized, cancellationToken);

    return await MapAsync(user, realm, cancellationToken);
  }

  public async Task<SearchResults<User>> SearchAsync(SearchUsersPayload payload, CancellationToken cancellationToken)
  {
    RealmEntity? realm = null;
    if (payload.Realm != null)
    {
      realm = await FindRealmAsync(payload.Realm, cancellationToken);
      if (realm == null)
      {
        return new SearchResults<User>();
      }
    }
    string? tenantId = realm?.AggregateId;

    IQueryBuilder builder = _queryHelper.From(Db.Users.Table)
      .Where(Db.Users.TenantId, tenantId == null ? Operators.IsNull() : Operators.IsEqualTo(tenantId))
      .SelectAll(Db.Users.Table);

    _queryHelper.ApplyTextSearch(builder, payload.Id, Db.Users.AggregateId);
    _queryHelper.ApplyTextSearch(builder, payload.Search, Db.Users.UniqueName, Db.Users.AddressFormatted,
      Db.Users.EmailAddress, Db.Users.PhoneE164Formatted, Db.Users.FirstName, Db.Users.MiddleName,
      Db.Users.LastName, Db.Users.Nickname, Db.Users.Gender, Db.Users.Locale, Db.Users.TimeZone);

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

    IQueryable<UserEntity> query = _users.FromQuery(builder.Build()).AsNoTracking();

    long total = await query.LongCountAsync(cancellationToken);

    int sortCount = payload.Sort.Count();
    if (sortCount > 0)
    {
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
              ? (sort.IsDescending ? query.OrderByDescending(x => x.PhoneNumber) : query.OrderBy(x => x.PhoneNumber))
              : (sort.IsDescending ? ordered.ThenByDescending(x => x.PhoneNumber) : ordered.ThenBy(x => x.PhoneNumber));
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

      if (ordered != null)
      {
        query = ordered;
      }
    }

    if (payload.Skip > 1)
    {
      query = query.Skip(payload.Skip);
    }
    if (payload.Limit > 1)
    {
      query = query.Take(payload.Limit);
    }

    UserEntity[] users = await query.ToArrayAsync(cancellationToken);
    IEnumerable<User> results = await MapAsync(users, realm, cancellationToken);

    return new SearchResults<User>(results, total);
  }

  private async Task<RealmEntity?> FindRealmAsync(string realm, CancellationToken cancellationToken)
  {
    string aggregateId = realm.Trim();
    string uniqueSlugNormalized = aggregateId.ToUpper();

    RealmEntity[] realms = await _realms.AsNoTracking()
      .Where(x => x.AggregateId == aggregateId || x.UniqueSlugNormalized == uniqueSlugNormalized)
      .ToArrayAsync(cancellationToken);

    return realms.Length > 1
      ? realms.FirstOrDefault(realm => realm.AggregateId == aggregateId)
      : realms.SingleOrDefault();
  }

  private async Task<User?> MapAsync(UserEntity? user, RealmEntity? realm, CancellationToken cancellationToken)
  {
    if (user == null)
    {
      return null;
    }

    return (await MapAsync(new[] { user }, realm, cancellationToken)).Single();
  }
  private async Task<IEnumerable<User>> MapAsync(IEnumerable<UserEntity> users, RealmEntity? realm, CancellationToken cancellationToken)
  {
    IEnumerable<ActorId> actorIds = ActorHelper.GetIds(users.ToArray());
    Dictionary<ActorId, Actor> actors = await _actorService.FindAsync(actorIds, cancellationToken);
    Mapper mapper = new(actors);

    Realm? mappedRealm = realm == null ? null : mapper.Map(realm);

    return users.Select(user => mapper.Map(user, mappedRealm));
  }
}
