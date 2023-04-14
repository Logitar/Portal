using Logitar.EventSourcing;
using Logitar.EventSourcing.EntityFrameworkCore.PostgreSQL;
using Logitar.Portal.Core.Caching;
using Logitar.Portal.Core.Configurations;

namespace Logitar.Portal.EntityFrameworkCore.PostgreSQL.Repositories;

internal class ConfigurationRepository : EventStore, IConfigurationRepository
{
  private readonly ICacheService _cacheService;

  public ConfigurationRepository(ICacheService cacheService,
    EventContext context,
    IEventBus eventBus) : base(context, eventBus)
  {
    _cacheService = cacheService;
  }

  public async Task<ConfigurationAggregate?> LoadAsync(CancellationToken cancellationToken)
  {
    return await LoadAsync<ConfigurationAggregate>(new AggregateId(Guid.Empty), cancellationToken);
  }

  public async Task SaveAsync(ConfigurationAggregate configuration, CancellationToken cancellationToken)
  {
    if (configuration.HasChanges)
    {
      _cacheService.Configuration = configuration;
    }

    await base.SaveAsync(configuration, cancellationToken);
  }
}
