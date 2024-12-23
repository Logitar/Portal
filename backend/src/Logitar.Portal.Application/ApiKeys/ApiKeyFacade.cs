using Logitar.Portal.Application.Activities;
using Logitar.Portal.Application.ApiKeys.Commands;
using Logitar.Portal.Application.ApiKeys.Queries;
using Logitar.Portal.Contracts.ApiKeys;
using Logitar.Portal.Contracts.Search;

namespace Logitar.Portal.Application.ApiKeys;

internal class ApiKeyFacade : IApiKeyService
{
  private readonly IActivityPipeline _activityPipeline;

  public ApiKeyFacade(IActivityPipeline activityPipeline)
  {
    _activityPipeline = activityPipeline;
  }

  public async Task<ApiKeyModel> AuthenticateAsync(AuthenticateApiKeyPayload payload, CancellationToken cancellationToken)
  {
    return await _activityPipeline.ExecuteAsync(new AuthenticateApiKeyCommand(payload), cancellationToken);
  }

  public async Task<ApiKeyModel> CreateAsync(CreateApiKeyPayload payload, CancellationToken cancellationToken)
  {
    return await _activityPipeline.ExecuteAsync(new CreateApiKeyCommand(payload), cancellationToken);
  }

  public async Task<ApiKeyModel?> DeleteAsync(Guid id, CancellationToken cancellationToken)
  {
    return await _activityPipeline.ExecuteAsync(new DeleteApiKeyCommand(id), cancellationToken);
  }

  public async Task<ApiKeyModel?> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    return await _activityPipeline.ExecuteAsync(new ReadApiKeyQuery(id), cancellationToken);
  }

  public async Task<ApiKeyModel?> ReplaceAsync(Guid id, ReplaceApiKeyPayload payload, long? version, CancellationToken cancellationToken)
  {
    return await _activityPipeline.ExecuteAsync(new ReplaceApiKeyCommand(id, payload, version), cancellationToken);
  }

  public async Task<SearchResults<ApiKeyModel>> SearchAsync(SearchApiKeysPayload payload, CancellationToken cancellationToken)
  {
    return await _activityPipeline.ExecuteAsync(new SearchApiKeysQuery(payload), cancellationToken);
  }

  public async Task<ApiKeyModel?> UpdateAsync(Guid id, UpdateApiKeyPayload payload, CancellationToken cancellationToken)
  {
    return await _activityPipeline.ExecuteAsync(new UpdateApiKeyCommand(id, payload), cancellationToken);
  }
}
