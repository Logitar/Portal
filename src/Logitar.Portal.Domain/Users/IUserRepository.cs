using Logitar.Portal.Contracts.Users;
using Logitar.Portal.Domain.Realms;

namespace Logitar.Portal.Domain.Users;

public interface IUserRepository
{
  Task<UserAggregate?> LoadAsync(Guid id, CancellationToken cancellationToken = default);
  Task<UserAggregate?> LoadAsync(string? tenantId, string uniqueName, CancellationToken cancellationToken = default);
  Task<IEnumerable<UserAggregate>> LoadAsync(RealmAggregate realm, CancellationToken cancellationToken = default);
  Task<IEnumerable<UserAggregate>> LoadAsync(string? tenantId, IEmailAddress email, CancellationToken cancellationToken = default);
  Task SaveAsync(UserAggregate user, CancellationToken cancellationToken = default);
  Task SaveAsync(IEnumerable<UserAggregate> users, CancellationToken cancellationToken = default);
}
