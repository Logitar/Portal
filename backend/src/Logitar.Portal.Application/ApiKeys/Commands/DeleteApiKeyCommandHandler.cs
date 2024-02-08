using Logitar.Identity.Domain.ApiKeys;
using Logitar.Portal.Contracts.ApiKeys;
using MediatR;

namespace Logitar.Portal.Application.ApiKeys.Commands;

internal class DeleteApiKeyCommandHandler : IRequestHandler<DeleteApiKeyCommand, ApiKey?>
{
  private readonly IApplicationContext _applicationContext;
  private readonly IApiKeyQuerier _apiKeyQuerier;
  private readonly IApiKeyRepository _apiKeyRepository;

  public DeleteApiKeyCommandHandler(IApplicationContext applicationContext, IApiKeyQuerier apiKeyQuerier, IApiKeyRepository apiKeyRepository)
  {
    _applicationContext = applicationContext;
    _apiKeyQuerier = apiKeyQuerier;
    _apiKeyRepository = apiKeyRepository;
  }

  public async Task<ApiKey?> Handle(DeleteApiKeyCommand command, CancellationToken cancellationToken)
  {
    ApiKeyAggregate? apiKey = await _apiKeyRepository.LoadAsync(command.Id, cancellationToken);
    if (apiKey == null || apiKey.TenantId != _applicationContext.TenantId)
    {
      return null;
    }
    ApiKey result = await _apiKeyQuerier.ReadAsync(apiKey, cancellationToken);

    apiKey.Delete(_applicationContext.ActorId);

    await _apiKeyRepository.SaveAsync(apiKey, cancellationToken);

    return result;
  }
}
