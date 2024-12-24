using Logitar.EventSourcing;
using Logitar.Portal.Application.Caching;
using Logitar.Portal.Application.Configurations;
using Logitar.Portal.Domain.Configurations;

namespace Logitar.Portal.EntityFrameworkCore.Relational.Repositories;

internal class ConfigurationRepository : Repository, IConfigurationRepository
{
  private readonly ICacheService _cacheService;
  private readonly IConfigurationQuerier _configurationQuerier;

  public ConfigurationRepository(ICacheService cacheService, IConfigurationQuerier configurationQuerier, IEventStore eventStore) : base(eventStore)
  {
    _cacheService = cacheService;
    _configurationQuerier = configurationQuerier;
  }

  public async Task<Configuration?> LoadAsync(CancellationToken cancellationToken)
    => await LoadAsync(version: null, cancellationToken);
  public async Task<Configuration?> LoadAsync(long? version, CancellationToken cancellationToken)
  {
    StreamId id = new ConfigurationId().StreamId;
    return await LoadAsync<Configuration>(id, version, cancellationToken);
  }

  public async Task SaveAsync(Configuration configuration, CancellationToken cancellationToken)
  {
    await base.SaveAsync(configuration, cancellationToken);
    _cacheService.Configuration = await _configurationQuerier.ReadAsync(configuration, cancellationToken);
  }
}
