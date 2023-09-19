using Logitar.Portal.Application.Roles.Commands;
using Logitar.Portal.Domain.ApiKeys;
using MediatR;

namespace Logitar.Portal.Application.ApiKeys.Commands;

internal class RemoveApiKeyRolesCommandHandler : INotificationHandler<RemoveRolesCommand>
{
  private readonly IApplicationContext _applicationContext;
  private readonly IApiKeyRepository _userRepository;

  public RemoveApiKeyRolesCommandHandler(IApplicationContext applicationContext, IApiKeyRepository userRepository)
  {
    _applicationContext = applicationContext;
    _userRepository = userRepository;
  }

  public async Task Handle(RemoveRolesCommand command, CancellationToken cancellationToken)
  {
    IEnumerable<ApiKeyAggregate> apiKeys = await _userRepository.LoadAsync(command.Role, cancellationToken);
    foreach (ApiKeyAggregate apiKey in apiKeys)
    {
      apiKey.RemoveRole(command.Role);
      apiKey.Update(_applicationContext.ActorId);
    }
    await _userRepository.SaveAsync(apiKeys, cancellationToken);
  }
}
