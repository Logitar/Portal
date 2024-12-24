using Logitar.Data;
using Logitar.EventSourcing;
using Logitar.Identity.Contracts.Users;
using Logitar.Identity.Core.Users;
using Logitar.Identity.EntityFrameworkCore.Relational;
using Logitar.Identity.EntityFrameworkCore.Relational.Entities;
using Logitar.Identity.EntityFrameworkCore.Relational.IdentityDb;
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
  private readonly IQueryHelper _queryHelper;
  private readonly DbSet<UserEntity> _users;

  public UserQuerier(IActorService actorService, IdentityContext context, IQueryHelper queryHelper)
  {
    _actorService = actorService;
    _queryHelper = queryHelper;
    _users = context.Users;
  }

  public async Task<UserModel> ReadAsync(RealmModel? realm, User user, CancellationToken cancellationToken)
  {
    return await ReadAsync(realm, user.Id, cancellationToken)
      ?? throw new InvalidOperationException($"The user entity 'StreamId={user.Id.Value}' could not be found.");
  }
  public async Task<UserModel?> ReadAsync(RealmModel? realm, UserId id, CancellationToken cancellationToken)
    => await ReadAsync(realm, id.EntityId.ToGuid(), cancellationToken);
  public async Task<UserModel?> ReadAsync(RealmModel? realm, Guid id, CancellationToken cancellationToken)
  {
    string streamId = new StreamId(id).Value;

    UserEntity? user = await _users.AsNoTracking()
      .Include(x => x.Identifiers)
      .Include(x => x.Roles)
      .SingleOrDefaultAsync(x => x.StreamId == streamId, cancellationToken);

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
    string uniqueNameNormalized = Helper.Normalize(uniqueName);

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

  public async Task<IEnumerable<UserModel>> ReadAsync(RealmModel? realm, IEmail email, CancellationToken cancellationToken)
  {
    string? tenantId = realm?.GetTenantId().Value;
    string emailAddressNormalized = Helper.Normalize(email.Address);

    UserEntity[] users = await _users.AsNoTracking()
      .Include(x => x.Identifiers)
      .Include(x => x.Roles)
      .Where(x => x.TenantId == tenantId && x.EmailAddressNormalized == emailAddressNormalized)
      .ToArrayAsync(cancellationToken);

    return await MapAsync(users, realm, cancellationToken);
  }

  public async Task<UserModel?> ReadAsync(RealmModel? realm, CustomIdentifierModel identifier, CancellationToken cancellationToken)
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
    IQueryBuilder builder = _queryHelper.QueryFrom(Users.Table).SelectAll(Users.Table)
      .ApplyRealmFilter(Users.TenantId, realm)
      .ApplyIdFilter(Users.StreamId, payload.Ids);
    _queryHelper.ApplyTextSearch(builder, payload.Search, Users.UniqueName, Users.AddressFormatted, Users.EmailAddress, Users.PhoneE164Formatted, Users.FirstName, Users.MiddleName, Users.LastName, Users.Nickname);

    if (payload.HasAuthenticated.HasValue)
    {
      NullOperator @operator = payload.HasAuthenticated.Value ? Operators.IsNotNull() : Operators.IsNull();
      builder.Where(Users.AuthenticatedOn, @operator);
    }
    if (payload.HasPassword.HasValue)
    {
      builder.Where(Users.HasPassword, Operators.IsEqualTo(payload.HasPassword.Value));
    }
    if (payload.IsDisabled.HasValue)
    {
      builder.Where(Users.IsDisabled, Operators.IsEqualTo(payload.IsDisabled.Value));
    }
    if (payload.IsConfirmed.HasValue)
    {
      builder.Where(Users.IsConfirmed, Operators.IsEqualTo(payload.IsConfirmed.Value));
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
    IEnumerable<UserModel> items = await MapAsync(users, realm, cancellationToken);

    return new SearchResults<UserModel>(items, total);
  }

  private async Task<UserModel> MapAsync(UserEntity user, RealmModel? realm, CancellationToken cancellationToken = default)
    => (await MapAsync([user], realm, cancellationToken)).Single();
  private async Task<IEnumerable<UserModel>> MapAsync(IEnumerable<UserEntity> users, RealmModel? realm, CancellationToken cancellationToken = default)
  {
    IEnumerable<ActorId> actorIds = users.SelectMany(user => user.GetActorIds());
    IEnumerable<ActorModel> actors = await _actorService.FindAsync(actorIds, cancellationToken);
    Mapper mapper = new(actors);

    return users.Select(user => mapper.ToUser(user, realm));
  }
}
