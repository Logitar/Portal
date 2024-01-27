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
    Dictionary<string, ApiKey> apiKeys = new(capacity: 1);

    if (!string.IsNullOrWhiteSpace(query.Id))
    {
      ApiKey? apiKey = await _apiKeyQuerier.ReadAsync(query.Id, cancellationToken);
      if (apiKey != null)
      {
        apiKeys[apiKey.Id] = apiKey;
      }
    }

    if (apiKeys.Count > 1)
    {
      throw new TooManyResultsException<ApiKey>(expectedCount: 1, actualCount: apiKeys.Count);
    }

    return apiKeys.Values.SingleOrDefault();
  }
}
