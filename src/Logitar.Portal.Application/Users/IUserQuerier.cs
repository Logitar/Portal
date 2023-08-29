using Logitar.Portal.Contracts.Users;
using Logitar.Portal.Domain.Users;

namespace Logitar.Portal.Application.Users;

public interface IUserQuerier
{
  Task<User> ReadAsync(UserAggregate user, CancellationToken cancellationToken = default);
  Task<User?> ReadAsync(Guid id, CancellationToken cancellationToken = default);
  Task<User?> ReadAsync(string? realm, string uniqueName, CancellationToken cancellationToken = default);
}
