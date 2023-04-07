using Logitar.Portal.v2.Contracts.Users;

namespace Logitar.Portal.v2.Core.Users;

public interface IUserQuerier
{
  Task<User> GetAsync(UserAggregate user, CancellationToken cancellationToken = default);
}
