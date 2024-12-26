using Logitar.Data;
using Logitar.EventSourcing;
using Logitar.Identity.Contracts.Users;
using Logitar.Identity.Domain.Users;
using Logitar.Identity.EntityFrameworkCore.Relational;
using Logitar.Identity.EntityFrameworkCore.Relational.Entities;
using Logitar.Portal.Application;
using Logitar.Portal.Application.Users;
using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Actors;
using Logitar.Portal.Contracts.Realms;
using Logitar.Portal.Contracts.Search;
using Logitar.Portal.Contracts.Users;
using Logitar.Portal.EntityFrameworkCore.Relational.Actors;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Portal.EntityFrameworkCore.Relational.Queriers;

internal class UserQuerier : IUserQuerier
{
  private readonly IActorService _actorService;
  private readonly ISearchHelper _searchHelper;
  private readonly ISqlHelper _sqlHelper;
  private readonly DbSet<UserEntity> _users;

  public UserQuerier(IActorService actorService, IdentityContext context, ISearchHelper searchHelper, ISqlHelper sqlHelper)
  {
    _actorService = actorService;
    _searchHelper = searchHelper;
    _sqlHelper = sqlHelper;
    _users = context.Users;
  }

  public async Task<UserModel> ReadAsync(RealmModel? realm, UserAggregate user, CancellationToken cancellationToken)
  {
    return await ReadAsync(realm, user.Id, cancellationToken)
      ?? throw new InvalidOperationException($"The user entity 'AggregateId={user.Id.Value}' could not be found.");
  }
  public async Task<UserModel?> ReadAsync(RealmModel? realm, UserId id, CancellationToken cancellationToken)
    => await ReadAsync(realm, id.ToGuid(), cancellationToken);
  public async Task<UserModel?> ReadAsync(RealmModel? realm, Guid id, CancellationToken cancellationToken)
  {
    string aggregateId = new AggregateId(id).Value;

    UserEntity? user = await _users.AsNoTracking()
      .Include(x => x.Identifiers)
      .Include(x => x.Roles)
      .SingleOrDefaultAsync(x => x.AggregateId == aggregateId, cancellationToken);

    string? tenantId = realm?.GetTenantId().Value;
    if (user == null || user.TenantId != tenantId)
    {
      return null;
    }

    return await MapAsync(user, realm, cancellationToken);
  }

  public async Task<UserModel?> ReadAsync(RealmModel? realm, string uniqueName, CancellationToken cancellationToken)
  {
    string? tenantId = realm?.GetTenantId().Value;
    string uniqueNameNormalized = uniqueName.Trim().ToUpper();

    UserEntity? user = await _users.AsNoTracking()
      .Include(x => x.Identifiers)
      .Include(x => x.Roles)
      .SingleOrDefaultAsync(x => x.TenantId == tenantId && x.UniqueNameNormalized == uniqueNameNormalized, cancellationToken);

    if (user == null)
    {
      return null;
    }

    return await MapAsync(user, realm, cancellationToken);
  }

  public async Task<IReadOnlyCollection<UserModel>> ReadAsync(RealmModel? realm, IEmail email, CancellationToken cancellationToken)
  {
    string? tenantId = realm?.GetTenantId().Value;
    string emailAddressNormalized = email.Address.Trim().ToUpper();

    UserEntity[] users = await _users.AsNoTracking()
      .Include(x => x.Identifiers)
      .Include(x => x.Roles)
      .Where(x => x.TenantId == tenantId && x.EmailAddressNormalized == emailAddressNormalized)
      .ToArrayAsync(cancellationToken);

    return await MapAsync(users, realm, cancellationToken);
  }

  public async Task<UserModel?> ReadAsync(RealmModel? realm, CustomIdentifier identifier, CancellationToken cancellationToken)
  {
    string? tenantId = realm?.GetTenantId().Value;
    string key = identifier.Key.Trim();
    string value = identifier.Value.Trim();

    UserEntity? user = await _users.AsNoTracking()
      .Include(x => x.Identifiers)
      .Include(x => x.Roles)
      .SingleOrDefaultAsync(x => x.TenantId == tenantId && x.Identifiers.Any(y => y.Key == key && y.Value == value), cancellationToken);

    if (user == null)
    {
      return null;
    }

    return await MapAsync(user, realm, cancellationToken);
  }

  public async Task<SearchResults<UserModel>> SearchAsync(RealmModel? realm, SearchUsersPayload payload, CancellationToken cancellationToken)
  {
    IQueryBuilder builder = _sqlHelper.QueryFrom(IdentityDb.Users.Table).SelectAll(IdentityDb.Users.Table)
      .ApplyRealmFilter(IdentityDb.Users.TenantId, realm)
      .ApplyIdFilter(IdentityDb.Users.AggregateId, payload.Ids);
    _searchHelper.ApplyTextSearch(builder, payload.Search, IdentityDb.Users.UniqueName, IdentityDb.Users.AddressFormatted, IdentityDb.Users.EmailAddress,
      IdentityDb.Users.PhoneE164Formatted, IdentityDb.Users.FirstName, IdentityDb.Users.MiddleName, IdentityDb.Users.LastName, IdentityDb.Users.Nickname);

    if (payload.HasAuthenticated.HasValue)
    {
      NullOperator @operator = payload.HasAuthenticated.Value ? Operators.IsNotNull() : Operators.IsNull();
      builder.Where(IdentityDb.Users.AuthenticatedOn, @operator);
    }
    if (payload.HasPassword.HasValue)
    {
      builder.Where(IdentityDb.Users.HasPassword, Operators.IsEqualTo(payload.HasPassword.Value));
    }
    if (payload.IsDisabled.HasValue)
    {
      builder.Where(IdentityDb.Users.IsDisabled, Operators.IsEqualTo(payload.IsDisabled.Value));
    }
    if (payload.IsConfirmed.HasValue)
    {
      builder.Where(IdentityDb.Users.IsConfirmed, Operators.IsEqualTo(payload.IsConfirmed.Value));
    }

    IQueryable<UserEntity> query = _users.FromQuery(builder).AsNoTracking()
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
    IReadOnlyCollection<UserModel> items = await MapAsync(users, realm, cancellationToken);

    return new SearchResults<UserModel>(items, total);
  }

  private async Task<UserModel> MapAsync(UserEntity user, RealmModel? realm, CancellationToken cancellationToken = default)
    => (await MapAsync([user], realm, cancellationToken)).Single();
  private async Task<IReadOnlyCollection<UserModel>> MapAsync(IEnumerable<UserEntity> users, RealmModel? realm, CancellationToken cancellationToken = default)
  {
    IReadOnlyCollection<ActorId> actorIds = users.SelectMany(user => user.GetActorIds()).ToArray();
    IReadOnlyCollection<ActorModel> actors = await _actorService.FindAsync(actorIds, cancellationToken);
    Mapper mapper = new(actors);

    return users.Select(user => mapper.ToUser(user, realm)).ToArray();
  }
}
