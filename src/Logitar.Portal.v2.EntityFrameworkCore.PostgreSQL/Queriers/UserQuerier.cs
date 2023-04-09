using AutoMapper;
using Logitar.EventSourcing;
using Logitar.Portal.v2.Contracts;
using Logitar.Portal.v2.Contracts.Users;
using Logitar.Portal.v2.Core.Users;
using Logitar.Portal.v2.EntityFrameworkCore.PostgreSQL.Entities;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Portal.v2.EntityFrameworkCore.PostgreSQL.Queriers;

internal class UserQuerier : IUserQuerier
{
  private readonly IMapper _mapper;
  private readonly DbSet<UserEntity> _users;

  public UserQuerier(PortalContext context, IMapper mapper)
  {
    _mapper = mapper;
    _users = context.Users;
  }

  public async Task<User> GetAsync(UserAggregate user, CancellationToken cancellationToken)
  {
    UserEntity? entity = await _users.AsNoTracking()
      .Include(x => x.ExternalIdentifiers)
      .Include(x => x.Realm)
      .SingleOrDefaultAsync(x => x.AggregateId == user.Id.Value, cancellationToken);

    return _mapper.Map<User>(entity);
  }

  public async Task<User?> GetAsync(Guid id, CancellationToken cancellationToken)
  {
    string aggregateId = new AggregateId(id).Value;

    UserEntity? user = await _users.AsNoTracking()
      .Include(x => x.ExternalIdentifiers)
      .Include(x => x.Realm)
      .SingleOrDefaultAsync(x => x.AggregateId == aggregateId, cancellationToken);

    return _mapper.Map<User>(user);
  }

  public async Task<User?> GetAsync(string realm, string username, CancellationToken cancellationToken)
  {
    string aggregateId = Guid.TryParse(realm, out Guid realmId)
      ? new AggregateId(realmId).Value
      : realm;

    UserEntity? user = await _users.AsNoTracking()
      .Include(x => x.ExternalIdentifiers)
      .Include(x => x.Realm)
      .SingleOrDefaultAsync(x => (x.Realm!.AggregateId == aggregateId || x.Realm.UniqueName == realm.ToUpper())
        && x.UsernameNormalized == username.ToUpper(), cancellationToken);

    return _mapper.Map<User>(user);
  }

  public async Task<User?> GetAsync(string realm, string externalKey, string externalValue, CancellationToken cancellationToken)
  {
    string aggregateId = Guid.TryParse(realm, out Guid realmId)
      ? new AggregateId(realmId).Value
      : realm;

    UserEntity? user = await _users.AsNoTracking()
      .Include(x => x.ExternalIdentifiers)
      .Include(x => x.Realm)
      .SingleOrDefaultAsync(x => (x.Realm!.AggregateId == aggregateId || x.Realm.UniqueName == realm.ToUpper())
        && x.ExternalIdentifiers.Any(y => y.Key == externalKey && y.ValueNormalized == externalValue), cancellationToken);

    return _mapper.Map<User>(user);
  }

  public async Task<PagedList<User>> GetAsync(bool? isConfirmed, bool? isDisabled, string? realm, string? search,
    UserSort? sort, bool isDescending, int? skip, int? limit, CancellationToken cancellationToken)
  {
    IQueryable<UserEntity> query = _users.AsNoTracking()
      .Include(x => x.ExternalIdentifiers)
      .Include(x => x.Realm);

    if (isConfirmed.HasValue)
    {
      query = query.Where(x => x.IsConfirmed == isConfirmed.Value);
    }
    if (isDisabled.HasValue)
    {
      query = query.Where(x => x.IsDisabled == isDisabled.Value);
    }
    if (realm != null)
    {
      string aggregateId = Guid.TryParse(realm, out Guid realmId)
        ? new AggregateId(realmId).Value
        : realm;

      query = query.Where(x => x.Realm!.AggregateId == aggregateId || x.Realm.UniqueNameNormalized == realm.ToUpper());
    }
    if (search != null)
    {
      foreach (string term in search.Split().Where(x => !string.IsNullOrEmpty(x)))
      {
        string pattern = $"%{term}%";

        query = query.Where(x => EF.Functions.ILike(x.Username, pattern)
          || EF.Functions.ILike(x.AddressFormatted!, pattern)
          || EF.Functions.ILike(x.EmailAddress!, pattern)
          || EF.Functions.ILike(x.PhoneE164Formatted!, pattern)
          || EF.Functions.ILike(x.FullName!, pattern)
          || EF.Functions.ILike(x.Nickname!, pattern));
      }
    }

    long total = await query.LongCountAsync(cancellationToken);

    if (sort.HasValue)
    {
      query = sort.Value switch
      {
        UserSort.DisabledOn => isDescending ? query.OrderByDescending(x => x.DisabledOn) : query.OrderBy(x => x.DisabledOn),
        UserSort.EmailAddress => isDescending ? query.OrderByDescending(x => x.EmailAddress) : query.OrderBy(x => x.EmailAddress),
        UserSort.FullName => isDescending ? query.OrderByDescending(x => x.FullName) : query.OrderBy(x => x.FullName),
        UserSort.LastFirstMiddleName => isDescending
          ? query.OrderByDescending(x => x.LastName).ThenByDescending(x => x.FirstName).ThenByDescending(x => x.MiddleName)
          : query.OrderBy(x => x.LastName).ThenBy(x => x.FirstName).ThenBy(x => x.MiddleName),
        UserSort.PasswordChangedOn => isDescending ? query.OrderByDescending(x => x.PasswordChangedOn) : query.OrderBy(x => x.PasswordChangedOn),
        UserSort.PhoneE164Formatted => isDescending ? query.OrderByDescending(x => x.PhoneE164Formatted) : query.OrderBy(x => x.PhoneE164Formatted),
        UserSort.SignedInOn => isDescending ? query.OrderByDescending(x => x.SignedInOn) : query.OrderBy(x => x.SignedInOn),
        UserSort.UpdatedOn => isDescending ? query.OrderByDescending(x => x.UpdatedOn ?? x.CreatedOn) : query.OrderBy(x => x.UpdatedOn ?? x.CreatedOn),
        UserSort.Username => isDescending ? query.OrderByDescending(x => x.Username) : query.OrderBy(x => x.Username),
        _ => throw new ArgumentException($"The user sort '{sort}' is not valid.", nameof(sort)),
      };
    }

    query = query.Page(skip, limit);

    UserEntity[] users = await query.ToArrayAsync(cancellationToken);

    return new PagedList<User>
    {
      Items = _mapper.Map<IEnumerable<User>>(users),
      Total = total
    };
  }
}
