using AutoMapper;
using Logitar.Data;
using Logitar.Identity.Domain.Users;
using Logitar.Identity.EntityFrameworkCore.Relational;
using Logitar.Identity.EntityFrameworkCore.Relational.Entities;
using Logitar.Portal.Application.Users;
using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Realms;
using Logitar.Portal.Contracts.Users;
using Logitar.Portal.EntityFrameworkCore.Relational.Entities;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Portal.EntityFrameworkCore.Relational.Queriers;

internal class UserQuerier : IUserQuerier
{
  private readonly IMapper _mapper;
  private readonly DbSet<RealmEntity> _realms;
  private readonly IPortalSqlHelper _sql;
  private readonly DbSet<UserEntity> _users;

  public UserQuerier(IdentityContext identityContext, IMapper mapper, PortalContext portalContext,
    IPortalSqlHelper sql)
  {
    _mapper = mapper;
    _realms = portalContext.Realms;
    _users = identityContext.Users;
    _sql = sql;
  }

  public async Task<User> ReadAsync(UserAggregate user, CancellationToken cancellationToken)
    => await ReadAsync(user.Id.Value, cancellationToken)
      ?? throw new InvalidOperationException($"The user entity 'AggregateId={user.Id}' could not be found.");
  public async Task<User?> ReadAsync(string id, CancellationToken cancellationToken)
  {
    UserEntity? user = await _users.AsNoTracking()
      .Include(x => x.Roles)
      .SingleOrDefaultAsync(x => x.AggregateId == id, cancellationToken);
    if (user == null)
    {
      return null;
    }

    RealmEntity? realm = null;
    if (user.TenantId != null)
    {
      realm = await _realms.AsNoTracking()
        .SingleOrDefaultAsync(x => x.AggregateId == user.TenantId, cancellationToken);
    }

    // TODO(fpion): Actors

    User result = _mapper.Map<User>(user);
    if (realm != null)
    {
      result.Realm = _mapper.Map<Realm>(realm);
    }

    return result;
  }

  public async Task<User?> ReadAsync(string? realmIdOrUniqueSlug, string uniqueName, CancellationToken cancellationToken)
  {
    RealmEntity? realm = null;
    if (!string.IsNullOrWhiteSpace(realmIdOrUniqueSlug))
    {
      string aggregateId = realmIdOrUniqueSlug.Trim();
      string uniqueSlugNormalized = aggregateId.ToUpper();

      realm = await _realms.AsNoTracking()
        .SingleOrDefaultAsync(x => x.AggregateId == aggregateId
          || x.UniqueSlugNormalized == uniqueSlugNormalized, cancellationToken);
      if (realm == null)
      {
        return null;
      }
    }

    string? tenantId = realm?.AggregateId;
    string uniqueNameNormalized = uniqueName.Trim().ToUpper();

    UserEntity? user = await _users.AsNoTracking()
      .SingleOrDefaultAsync(x => x.TenantId == tenantId && x.UniqueNameNormalized == uniqueNameNormalized, cancellationToken);
    if (user == null)
    {
      return null;
    }

    // TODO(fpion): Actors

    User result = _mapper.Map<User>(user);
    if (realm != null)
    {
      result.Realm = _mapper.Map<Realm>(realm);
    }

    return result;
  }

  public async Task<SearchResults<User>> SearchAsync(SearchUsersPayload payload, CancellationToken cancellationToken)
  {
    RealmEntity? realm = null;
    if (!string.IsNullOrWhiteSpace(payload.Realm))
    {
      string aggregateId = payload.Realm.Trim();
      string uniqueSlugNormalized = aggregateId.ToUpper();

      realm = await _realms.AsNoTracking()
        .SingleOrDefaultAsync(x => x.AggregateId == aggregateId
          || x.UniqueSlugNormalized == uniqueSlugNormalized, cancellationToken);
      if (realm == null)
      {
        return new SearchResults<User>();
      }
    }

    IQueryBuilder builder = _sql.QueryFrom(Db.Users.Table)
      .Where(Db.Users.TenantId, realm == null ? Operators.IsNull() : Operators.IsEqualTo(realm.AggregateId))
      .SelectAll(Db.Users.Table);
    _sql.ApplyTextSearch(builder, payload.Id, Db.Users.AggregateId);
    _sql.ApplyTextSearch(builder, payload.Search, Db.Users.UniqueName, Db.Users.AddressFormatted,
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

    if (payload.Sort.Any())
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
          case UserSort.FullName:
            ordered = (ordered == null)
              ? (sort.IsDescending ? query.OrderByDescending(x => x.FullName) : query.OrderBy(x => x.FullName))
              : (sort.IsDescending ? ordered.ThenByDescending(x => x.FullName) : ordered.ThenBy(x => x.FullName));
            break;
          case UserSort.LastFirstMiddleName:
            ordered = (ordered == null)
              ? (sort.IsDescending
                ? query.OrderByDescending(x => x.LastName).ThenByDescending(x => x.FirstName).ThenByDescending(x => x.MiddleName)
                : query.OrderBy(x => x.LastName).ThenBy(x => x.FirstName).ThenBy(x => x.MiddleName)
              ) : (sort.IsDescending
                ? ordered.ThenByDescending(x => x.LastName).ThenByDescending(x => x.FirstName).ThenByDescending(x => x.FirstName)
                : ordered.ThenBy(x => x.LastName).ThenBy(x => x.FirstName).ThenBy(x => x.MiddleName)
              );
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
    }

    query = query.ApplyPaging(payload);

    UserEntity[] users = await query.ToArrayAsync(cancellationToken);

    User[] results = _mapper.Map<IEnumerable<User>>(users).ToArray();
    if (realm != null)
    {
      Realm realmResult = _mapper.Map<Realm>(realm);
      foreach (User result in results)
      {
        result.Realm = realmResult;
      }
    }

    // TODO(fpion): Actors

    return new SearchResults<User>(results, total);
  }
}
