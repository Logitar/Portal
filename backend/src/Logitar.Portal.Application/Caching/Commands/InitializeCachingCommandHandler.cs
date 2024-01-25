using Logitar.Portal.Application.Configurations;
using Logitar.Portal.Contracts.Configurations;
using MediatR;

namespace Logitar.Portal.Application.Caching.Commands;

internal class InitializeCachingCommandHandler : INotificationHandler<InitializeCachingCommand>
{
  private readonly ICacheService _cacheService;
  private readonly IConfigurationQuerier _configurationQuerier;

  public InitializeCachingCommandHandler(ICacheService cacheService, IConfigurationQuerier configurationQuerier)
  {
    _cacheService = cacheService;
    _configurationQuerier = configurationQuerier;
  }

  public async Task Handle(InitializeCachingCommand _, CancellationToken cancellationToken)
  {
    Configuration? configuration = await _configurationQuerier.ReadAsync(cancellationToken);
    if (configuration != null)
    {
      _cacheService.SetConfiguration(configuration);
    }
  }
}
