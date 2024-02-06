using Logitar.Identity.Domain.Users;
using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Search;
using Logitar.Portal.Contracts.Users;

namespace Logitar.Portal.Application.Users;

public interface IUserQuerier
{
  Task<User> ReadAsync(UserAggregate session, CancellationToken cancellationToken = default);
  Task<User?> ReadAsync(UserId id, CancellationToken cancellationToken = default);
  Task<User?> ReadAsync(Guid id, CancellationToken cancellationToken = default);
  Task<User?> ReadAsync(string uniqueName, CancellationToken cancellationToken = default);
  Task<User?> ReadAsync(CustomIdentifier identifier, CancellationToken cancellationToken = default);
  Task<SearchResults<User>> SearchAsync(SearchUsersPayload payload, CancellationToken cancellationToken = default);
}
