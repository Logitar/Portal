using Logitar.Portal.Application.ApiKeys.Commands;
using Logitar.Portal.Application.ApiKeys.Queries;
using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.ApiKeys;

namespace Logitar.Portal.Application.ApiKeys
{
  internal class ApiKeyService : IApiKeyService
  {
    private readonly IRequestPipeline _requestPipeline;

    public ApiKeyService(IRequestPipeline requestPipeline)
    {
      _requestPipeline = requestPipeline;
    }

    public async Task<ApiKeyModel> CreateAsync(CreateApiKeyPayload payload, CancellationToken cancellationToken)
    {
      return await _requestPipeline.ExecuteAsync(new CreateApiKeyCommand(payload), cancellationToken);
    }

    public async Task DeleteAsync(string id, CancellationToken cancellationToken)
    {
      await _requestPipeline.ExecuteAsync(new DeleteApiKeyCommand(id), cancellationToken);
    }

    public async Task<ApiKeyModel?> GetAsync(string id, CancellationToken cancellationToken)
    {
      return await _requestPipeline.ExecuteAsync(new GetApiKeyQuery(id), cancellationToken);
    }

    public async Task<ListModel<ApiKeyModel>> GetAsync(DateTime? expiredOn, string? search,
      ApiKeySort? sort, bool isDescending,
      int? index, int? count,
      CancellationToken cancellationToken)
    {
      return await _requestPipeline.ExecuteAsync(new GetApiKeysQuery(expiredOn, search,
        sort, isDescending,
        index, count), cancellationToken);
    }

    public async Task<ApiKeyModel> UpdateAsync(string id, UpdateApiKeyPayload payload, CancellationToken cancellationToken)
    {
      return await _requestPipeline.ExecuteAsync(new UpdateApiKeyCommand(id, payload), cancellationToken);
    }
  }
}
