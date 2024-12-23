using Logitar.Portal.Application.Activities;
using Logitar.Portal.Contracts.ApiKeys;
using MediatR;

namespace Logitar.Portal.Application.ApiKeys.Queries;

internal record ReadApiKeyQuery(Guid Id) : Activity, IRequest<ApiKeyModel?>;

internal class ReadApiKeyQueryHandler : IRequestHandler<ReadApiKeyQuery, ApiKeyModel?>
{
  private readonly IApiKeyQuerier _apiKeyQuerier;

  public ReadApiKeyQueryHandler(IApiKeyQuerier apiKeyQuerier)
  {
    _apiKeyQuerier = apiKeyQuerier;
  }

  public async Task<ApiKeyModel?> Handle(ReadApiKeyQuery query, CancellationToken cancellationToken)
  {
    return await _apiKeyQuerier.ReadAsync(query.Realm, query.Id, cancellationToken);
  }
}
