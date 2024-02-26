using Logitar.Identity.Domain.Sessions;

namespace Logitar.Portal.Application.Sessions;

internal static class SessionRepositoryExtensions
{
  public static async Task<SessionAggregate?> LoadAsync(this ISessionRepository repository, Guid id, CancellationToken cancellationToken = default)
  {
    SessionId sessionId = new(id);
    return await repository.LoadAsync(sessionId, cancellationToken);
  }
}
