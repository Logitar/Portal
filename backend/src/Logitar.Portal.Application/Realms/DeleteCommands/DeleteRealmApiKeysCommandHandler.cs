using Logitar.Identity.Domain.ApiKeys;
using Logitar.Identity.Domain.Shared;
using MediatR;

namespace Logitar.Portal.Application.Realms.DeleteCommands;

internal class DeleteRealmApiKeysCommandHandler : INotificationHandler<DeleteRealmApiKeysCommand>
{
  private readonly IApiKeyRepository _apiKeyRepository;

  public DeleteRealmApiKeysCommandHandler(IApiKeyRepository apiKeyRepository)
  {
    _apiKeyRepository = apiKeyRepository;
  }

  public async Task Handle(DeleteRealmApiKeysCommand command, CancellationToken cancellationToken)
  {
    TenantId tenantId = new(command.Realm.Id.Value);
    IEnumerable<ApiKey> apiKeys = await _apiKeyRepository.LoadAsync(tenantId, cancellationToken);

    foreach (ApiKey apiKey in apiKeys)
    {
      apiKey.Delete(command.ActorId);
    }

    await _apiKeyRepository.SaveAsync(apiKeys, cancellationToken);
  }
}
