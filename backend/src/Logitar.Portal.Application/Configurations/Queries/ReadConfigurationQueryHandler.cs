using Logitar.Portal.Application.Caching;
using Logitar.Portal.Contracts.Configurations;
using Logitar.Portal.Domain.Configurations;
using MediatR;

namespace Logitar.Portal.Application.Configurations.Queries;

internal class ReadConfigurationQueryHandler : IRequestHandler<ReadConfigurationQuery, Configuration?>
{
  private readonly ICacheService _cacheService;
  private readonly IConfigurationQuerier _configurationQuerier;

  public ReadConfigurationQueryHandler(ICacheService cacheService, IConfigurationQuerier configurationQuerier)
  {
    _cacheService = cacheService;
    _configurationQuerier = configurationQuerier;
  }

  public async Task<Configuration?> Handle(ReadConfigurationQuery _, CancellationToken cancellationToken)
  {
    ConfigurationAggregate? configuration = _cacheService.GetConfiguration();
    return configuration == null ? null : await _configurationQuerier.ReadAsync(configuration, cancellationToken);
  }
}
