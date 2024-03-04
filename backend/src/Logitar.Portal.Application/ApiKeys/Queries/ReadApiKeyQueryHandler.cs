using Logitar.Portal.Contracts.ApiKeys;
using MediatR;

namespace Logitar.Portal.Application.ApiKeys.Queries;

internal class ReadApiKeyQueryHandler : IRequestHandler<ReadApiKeyQuery, ApiKey?>
{
  private readonly IApiKeyQuerier _apiKeyQuerier;

  public ReadApiKeyQueryHandler(IApiKeyQuerier apiKeyQuerier)
  {
    _apiKeyQuerier = apiKeyQuerier;
  }

  public async Task<ApiKey?> Handle(ReadApiKeyQuery query, CancellationToken cancellationToken)
  {
    return await _apiKeyQuerier.ReadAsync(query.Realm, query.Id, cancellationToken);
  }
}
