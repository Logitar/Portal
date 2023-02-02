using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.ApiKeys;
using MediatR;

namespace Logitar.Portal.Application.ApiKeys.Queries
{
  internal class GetApiKeysQueryHandler : IRequestHandler<GetApiKeysQuery, ListModel<ApiKeyModel>>
  {
    private readonly IApiKeyQuerier _apiKeyQuerier;

    public GetApiKeysQueryHandler(IApiKeyQuerier apiKeyQuerier)
    {
      _apiKeyQuerier = apiKeyQuerier;
    }

    public async Task<ListModel<ApiKeyModel>> Handle(GetApiKeysQuery request, CancellationToken cancellationToken)
    {
      return await _apiKeyQuerier.GetPagedAsync(request.ExpiredOn, request.Search,
        request.Sort, request.IsDescending,
        request.Index, request.Count,
        cancellationToken);
    }
  }
}
