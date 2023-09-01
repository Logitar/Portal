using Logitar.Portal.Domain.ApiKeys;
using MediatR;

namespace Logitar.Portal.Application.ApiKeys.Commands;

internal class DeleteApiKeysCommandHandler : INotificationHandler<DeleteApiKeysCommand>
{
  private readonly IApplicationContext _applicationContext;
  private readonly IApiKeyRepository _userRepository;

  public DeleteApiKeysCommandHandler(IApplicationContext applicationContext, IApiKeyRepository userRepository)
  {
    _applicationContext = applicationContext;
    _userRepository = userRepository;
  }

  public async Task Handle(DeleteApiKeysCommand command, CancellationToken cancellationToken)
  {
    IEnumerable<ApiKeyAggregate> apiKeys = await _userRepository.LoadAsync(command.Realm, cancellationToken);
    foreach (ApiKeyAggregate apiKey in apiKeys)
    {
      apiKey.Delete(_applicationContext.ActorId);
    }

    await _userRepository.SaveAsync(apiKeys, cancellationToken);
  }
}
