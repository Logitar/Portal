using Logitar.Portal.Domain.Configurations;
using MediatR;

namespace Logitar.Portal.Application.Caching.Commands;

internal class InitializeCacheCommandHandler : IRequestHandler<InitializeCacheCommand, Unit>
{
  private readonly ICacheService _cacheService;
  private readonly IConfigurationRepository _configurationRepository;

  public InitializeCacheCommandHandler(ICacheService cacheService,
    IConfigurationRepository configurationRepository)
  {
    _cacheService = cacheService;
    _configurationRepository = configurationRepository;
  }

  public async Task<Unit> Handle(InitializeCacheCommand _, CancellationToken cancellationToken)
  {
    ConfigurationAggregate? configuration = await _configurationRepository.LoadAsync(cancellationToken);
    if (configuration != null)
    {
      _cacheService.Configuration = configuration;
    }

    return Unit.Value;
  }
}
