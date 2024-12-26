using Logitar.EventSourcing;
using Logitar.Identity.Domain.ApiKeys;
using Logitar.Identity.Domain.Shared;
using Logitar.Portal.Domain.Realms;
using MediatR;

namespace Logitar.Portal.Application.Realms.DeleteCommands;

internal record DeleteRealmApiKeysCommand(Realm Realm, ActorId ActorId) : INotification;

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
    IEnumerable<ApiKeyAggregate> apiKeys = await _apiKeyRepository.LoadAsync(tenantId, cancellationToken);

    foreach (ApiKeyAggregate apiKey in apiKeys)
    {
      apiKey.Delete(command.ActorId);
    }

    await _apiKeyRepository.SaveAsync(apiKeys, cancellationToken);
  }
}
