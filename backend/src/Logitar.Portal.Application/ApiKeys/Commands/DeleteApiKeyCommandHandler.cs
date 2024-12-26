using Logitar.Identity.Domain.ApiKeys;
using Logitar.Portal.Contracts.ApiKeys;
using MediatR;

namespace Logitar.Portal.Application.ApiKeys.Commands;

internal class DeleteApiKeyCommandHandler : IRequestHandler<DeleteApiKeyCommand, ApiKeyModel?>
{
  private readonly IApiKeyQuerier _apiKeyQuerier;
  private readonly IApiKeyRepository _apiKeyRepository;

  public DeleteApiKeyCommandHandler(IApiKeyQuerier apiKeyQuerier, IApiKeyRepository apiKeyRepository)
  {
    _apiKeyQuerier = apiKeyQuerier;
    _apiKeyRepository = apiKeyRepository;
  }

  public async Task<ApiKeyModel?> Handle(DeleteApiKeyCommand command, CancellationToken cancellationToken)
  {
    ApiKeyAggregate? apiKey = await _apiKeyRepository.LoadAsync(command.Id, cancellationToken);
    if (apiKey == null || apiKey.TenantId != command.TenantId)
    {
      return null;
    }
    ApiKeyModel result = await _apiKeyQuerier.ReadAsync(command.Realm, apiKey, cancellationToken);

    apiKey.Delete(command.ActorId);

    await _apiKeyRepository.SaveAsync(apiKey, cancellationToken);

    return result;
  }
}
