namespace Logitar.Portal.Contracts.ApiKeys;

public interface IApiKeyService
{
  Task<ApiKey> AuthenticateAsync(AuthenticateApiKeyPayload payload, CancellationToken cancellationToken = default);
  Task<ApiKey> CreateAsync(CreateApiKeyPayload payload, CancellationToken cancellationToken = default);
  Task<ApiKey?> DeleteAsync(string id, CancellationToken cancellationToken = default);
  Task<ApiKey?> ReadAsync(string? id = null, CancellationToken cancellationToken = default);
  Task<ApiKey?> ReplaceAsync(string id, ReplaceApiKeyPayload payload, long? version = null, CancellationToken cancellationToken = default);
  Task<ApiKey?> UpdateAsync(string id, UpdateApiKeyPayload payload, CancellationToken cancellationToken = default);
}
