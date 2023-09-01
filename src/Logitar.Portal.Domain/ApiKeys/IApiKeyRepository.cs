namespace Logitar.Portal.Domain.ApiKeys;

public interface IApiKeyRepository
{
  Task SaveAsync(ApiKeyAggregate apiKey, CancellationToken cancellationToken = default);
}
