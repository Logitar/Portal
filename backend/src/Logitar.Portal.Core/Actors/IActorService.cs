using Logitar.Portal.Core.ApiKeys;
using Logitar.Portal.Core.Users;

namespace Logitar.Portal.Core.Actors
{
  public interface IActorService
  {
    Task<Actor?> GetAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Actor>> GetAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken = default);
    Task SaveAsync(ApiKey apiKey, CancellationToken cancellationToken = default);
    Task SaveAsync(User user, CancellationToken cancellationToken = default);
    Task SaveAsync(IEnumerable<ApiKey> apiKeys, CancellationToken cancellationToken = default);
    Task SaveAsync(IEnumerable<User> users, CancellationToken cancellationToken = default);
    Task SynchronizeAsync(CancellationToken cancellationToken = default);
  }
}
