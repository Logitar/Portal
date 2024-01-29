using Logitar.Portal.Application.Configurations;
using Logitar.Portal.Domain.Configurations;
using MediatR;

namespace Logitar.Portal.Application.Caching.Commands;

internal class InitializeCachingCommandHandler : INotificationHandler<InitializeCachingCommand>
{
  private readonly ICacheService _cacheService;
  private readonly IConfigurationQuerier _configurationQuerier;
  private readonly IConfigurationRepository _configurationRepository;

  public InitializeCachingCommandHandler(ICacheService cacheService, IConfigurationQuerier configurationQuerier, IConfigurationRepository configurationRepository)
  {
    _cacheService = cacheService;
    _configurationQuerier = configurationQuerier;
    _configurationRepository = configurationRepository;
  }

  public async Task Handle(InitializeCachingCommand _, CancellationToken cancellationToken)
  {
    ConfigurationAggregate? configuration = await _configurationRepository.LoadAsync(cancellationToken);
    if (configuration != null)
    {
      _cacheService.Configuration = await _configurationQuerier.ReadAsync(configuration, cancellationToken);
    }
  }
}
