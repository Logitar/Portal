using Logitar.Portal.Contracts.Users;
using Logitar.Portal.Domain.Users;

namespace Logitar.Portal.Application.Users;

public interface IUserQuerier
{
  Task<User> ReadAsync(UserAggregate user, CancellationToken cancellationToken = default);
}
