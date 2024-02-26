using Logitar.Identity.Domain.Passwords;

namespace Logitar.Portal.Application.OneTimePasswords;

internal static class OneTimePasswordRepositoryExtensions
{
  public static async Task<OneTimePasswordAggregate?> LoadAsync(this IOneTimePasswordRepository repository, Guid id, CancellationToken cancellationToken = default)
  {
    OneTimePasswordId sessionId = new(id);
    return await repository.LoadAsync(sessionId, cancellationToken);
  }
}
