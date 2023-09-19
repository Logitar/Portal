using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.ApiKeys;
using MediatR;

namespace Logitar.Portal.Application.ApiKeys.Queries;

internal class SearchApiKeysQueryHandler : IRequestHandler<SearchApiKeysQuery, SearchResults<ApiKey>>
{
  private readonly IApiKeyQuerier _apiKeyQuerier;

  public SearchApiKeysQueryHandler(IApiKeyQuerier apiKeyQuerier)
  {
    _apiKeyQuerier = apiKeyQuerier;
  }

  public async Task<SearchResults<ApiKey>> Handle(SearchApiKeysQuery query, CancellationToken cancellationToken)
  {
    return await _apiKeyQuerier.SearchAsync(query.Payload, cancellationToken);
  }
}
