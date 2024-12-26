using Logitar.Portal.Application.Caching;
using Logitar.Portal.Contracts.Configurations;
using MediatR;

namespace Logitar.Portal.Application.Configurations.Queries;

internal class ReadConfigurationQueryHandler : IRequestHandler<ReadConfigurationQuery, ConfigurationModel>
{
  private readonly ICacheService _cacheService;

  public ReadConfigurationQueryHandler(ICacheService cacheService)
  {
    _cacheService = cacheService;
  }

  public Task<ConfigurationModel> Handle(ReadConfigurationQuery _, CancellationToken cancellationToken)
  {
    ConfigurationModel configuration = _cacheService.Configuration ?? throw new InvalidOperationException("The configuration should be in the cache.");
    return Task.FromResult(configuration);
  }
}
