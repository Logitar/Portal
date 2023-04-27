using Logitar.Portal.Core.Configurations;
using MediatR;

namespace Logitar.Portal.Core.Caching.Commands;

internal class InitializeCachingHandler : INotificationHandler<InitializeCaching>
{
  private readonly ICacheService _cacheService;
  private readonly IConfigurationRepository _configurationRepository;

  public InitializeCachingHandler(ICacheService cacheService, IConfigurationRepository configurationRepository)
  {
    _cacheService = cacheService;
    _configurationRepository = configurationRepository;
  }

  public async Task Handle(InitializeCaching notification, CancellationToken cancellationToken)
  {
    ConfigurationAggregate? configuration = await _configurationRepository.LoadAsync(cancellationToken);
    if (configuration != null)
    {
      _cacheService.Configuration = configuration;
    }
  }
}
