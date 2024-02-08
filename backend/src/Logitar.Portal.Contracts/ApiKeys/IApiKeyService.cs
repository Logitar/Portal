namespace Logitar.Portal.Contracts.ApiKeys;

public interface IApiKeyService
{
  Task<ApiKey> CreateAsync(CreateApiKeyPayload payload, CancellationToken cancellationToken = default);
  Task<ApiKey?> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
  Task<ApiKey?> ReadAsync(Guid id, CancellationToken cancellationToken = default);
  Task<ApiKey?> ReplaceAsync(Guid id, ReplaceApiKeyPayload payload, long? version = null, CancellationToken cancellationToken = default);
}
