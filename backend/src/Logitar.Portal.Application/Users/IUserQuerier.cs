using Logitar.Identity.Domain.Users;
using Logitar.Portal.Contracts.Users;

namespace Logitar.Portal.Application.Users;

public interface IUserQuerier
{
  Task<User> ReadAsync(UserAggregate user, CancellationToken cancellationToken = default);
  Task<User?> ReadAsync(UserId id, CancellationToken cancellationToken = default);
  Task<User?> ReadAsync(string id, CancellationToken cancellationToken = default);
}
