using Logitar.Identity.Domain.ApiKeys;

namespace Logitar.Portal.Application.ApiKeys;

internal static class ApiKeyRepositoryExtensions
{
  public static async Task<ApiKeyAggregate?> LoadAsync(this IApiKeyRepository repository, Guid id, CancellationToken cancellationToken = default)
  {
    ApiKeyId sessionId = new(id);
    return await repository.LoadAsync(sessionId, cancellationToken);
  }
}
