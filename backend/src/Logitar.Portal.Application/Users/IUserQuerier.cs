using Logitar.Identity.Contracts.Users;
using Logitar.Identity.Domain.Users;
using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Realms;
using Logitar.Portal.Contracts.Search;
using Logitar.Portal.Contracts.Users;

namespace Logitar.Portal.Application.Users;

public interface IUserQuerier
{
  Task<User> ReadAsync(Realm? realm, UserAggregate session, CancellationToken cancellationToken = default);
  Task<User?> ReadAsync(Realm? realm, UserId id, CancellationToken cancellationToken = default);
  Task<User?> ReadAsync(Realm? realm, Guid id, CancellationToken cancellationToken = default);
  Task<User?> ReadAsync(Realm? realm, string uniqueName, CancellationToken cancellationToken = default);
  Task<IEnumerable<User>> ReadAsync(Realm? realm, IEmail email, CancellationToken cancellationToken = default);
  Task<User?> ReadAsync(Realm? realm, CustomIdentifierModel identifier, CancellationToken cancellationToken = default);
  Task<SearchResults<User>> SearchAsync(Realm? realm, SearchUsersPayload payload, CancellationToken cancellationToken = default);
}
