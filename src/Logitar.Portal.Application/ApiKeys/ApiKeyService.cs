using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.ApiKeys;

namespace Logitar.Portal.Application.ApiKeys;

internal class ApiKeyService : IApiKeyService
{
  public Task<ApiKey> AuthenticateAsync(string xApiKey, CancellationToken cancellationToken)
  {
    throw new NotImplementedException(); // TODO(fpion): implement
  }

  public Task<ApiKey> CreateAsync(CreateApiKeyPayload payload, CancellationToken cancellationToken)
  {
    throw new NotImplementedException(); // TODO(fpion): implement
  }

  public Task<ApiKey?> DeleteAsync(Guid id, CancellationToken cancellationToken)
  {
    throw new NotImplementedException(); // TODO(fpion): implement
  }

  public Task<ApiKey?> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    throw new NotImplementedException(); // TODO(fpion): implement
  }

  public Task<ApiKey?> ReplaceAsync(Guid id, ReplaceApiKeyPayload payload, long? version, CancellationToken cancellationToken)
  {
    throw new NotImplementedException(); // TODO(fpion): implement
  }

  public Task<SearchResults<ApiKey>> SearchAsync(SearchApiKeysPayload payload, CancellationToken cancellationToken)
  {
    throw new NotImplementedException(); // TODO(fpion): implement
  }

  public Task<ApiKey?> UpdateAsync(Guid id, UpdateApiKeyPayload payload, CancellationToken cancellationToken)
  {
    throw new NotImplementedException(); // TODO(fpion): implement
  }
}
