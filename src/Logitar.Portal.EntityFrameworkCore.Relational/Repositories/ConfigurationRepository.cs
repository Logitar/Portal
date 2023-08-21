using Logitar.EventSourcing.EntityFrameworkCore.Relational;
using Logitar.EventSourcing.Infrastructure;
using Logitar.Portal.Application.Caching;
using Logitar.Portal.Domain.Configurations;

namespace Logitar.Portal.EntityFrameworkCore.Relational.Repositories;

internal class ConfigurationRepository : EventSourcing.EntityFrameworkCore.Relational.AggregateRepository, IConfigurationRepository
{
  private readonly ICacheService _cacheService;

  public ConfigurationRepository(ICacheService cacheService, IEventBus eventBus,
    EventContext eventContext, IEventSerializer eventSerializer)
      : base(eventBus, eventContext, eventSerializer)
  {
    _cacheService = cacheService;
  }

  public async Task<ConfigurationAggregate?> LoadAsync(CancellationToken cancellationToken)
    => await base.LoadAsync<ConfigurationAggregate>(ConfigurationAggregate.UniqueId, cancellationToken);

  public async Task SaveAsync(ConfigurationAggregate configuration, CancellationToken cancellationToken)
  {
    await base.SaveAsync(configuration, cancellationToken);

    _cacheService.Configuration = configuration;
  }
}
