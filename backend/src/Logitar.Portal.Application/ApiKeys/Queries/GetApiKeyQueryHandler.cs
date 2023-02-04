using Logitar.Portal.Contracts.ApiKeys;
using MediatR;

namespace Logitar.Portal.Application.ApiKeys.Queries
{
  internal class GetApiKeyQueryHandler : IRequestHandler<GetApiKeyQuery, ApiKeyModel?>
  {
    private readonly IApiKeyQuerier _apiKeyQuerier;

    public GetApiKeyQueryHandler(IApiKeyQuerier apiKeyQuerier)
    {
      _apiKeyQuerier = apiKeyQuerier;
    }

    public async Task<ApiKeyModel?> Handle(GetApiKeyQuery request, CancellationToken cancellationToken)
    {
      return await _apiKeyQuerier.GetAsync(request.Id, cancellationToken);
    }
  }
}
