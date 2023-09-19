using Logitar.Portal.Domain.ApiKeys;
using MediatR;

namespace Logitar.Portal.Application.ApiKeys.Commands;

internal class DeleteApiKeysCommandHandler : INotificationHandler<DeleteApiKeysCommand>
{
  private readonly IApiKeyRepository _apiKeyRepository;
  private readonly IApplicationContext _applicationContext;

  public DeleteApiKeysCommandHandler(IApiKeyRepository apiKeyRepository, IApplicationContext applicationContext)
  {
    _apiKeyRepository = apiKeyRepository;
    _applicationContext = applicationContext;
  }

  public async Task Handle(DeleteApiKeysCommand command, CancellationToken cancellationToken)
  {
    IEnumerable<ApiKeyAggregate> apiKeys = await _apiKeyRepository.LoadAsync(command.Realm, cancellationToken);
    foreach (ApiKeyAggregate apiKey in apiKeys)
    {
      apiKey.Delete(_applicationContext.ActorId);
    }

    await _apiKeyRepository.SaveAsync(apiKeys, cancellationToken);
  }
}
