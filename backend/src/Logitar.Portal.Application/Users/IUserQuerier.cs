using Logitar.Identity.Contracts.Users;
using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Realms;
using Logitar.Portal.Contracts.Search;
using Logitar.Portal.Contracts.Users;

namespace Logitar.Portal.Application.Users;

public interface IUserQuerier
{
  Task<User> ReadAsync(RealmModel? realm, UserAggregate session, CancellationToken cancellationToken = default);
  Task<User?> ReadAsync(RealmModel? realm, UserId id, CancellationToken cancellationToken = default);
  Task<User?> ReadAsync(RealmModel? realm, Guid id, CancellationToken cancellationToken = default);
  Task<User?> ReadAsync(RealmModel? realm, string uniqueName, CancellationToken cancellationToken = default);
  Task<IReadOnlyCollection<User>> ReadAsync(RealmModel? realm, IEmail email, CancellationToken cancellationToken = default);
  Task<User?> ReadAsync(RealmModel? realm, CustomIdentifier identifier, CancellationToken cancellationToken = default);
  Task<SearchResults<User>> SearchAsync(RealmModel? realm, SearchUsersPayload payload, CancellationToken cancellationToken = default);
}
