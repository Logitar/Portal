using Logitar.EventSourcing;
using Logitar.EventSourcing.EntityFrameworkCore.Relational;
using Logitar.EventSourcing.Infrastructure;
using Logitar.Portal.Application.Caching;
using Logitar.Portal.Application.Configurations;
using Logitar.Portal.Domain.Configurations;

namespace Logitar.Portal.EntityFrameworkCore.Relational.Repositories;

internal class ConfigurationRepository : EventSourcing.EntityFrameworkCore.Relational.AggregateRepository, IConfigurationRepository
{
  private readonly ICacheService _cacheService;
  private readonly IConfigurationQuerier _configurationQuerier;

  public ConfigurationRepository(ICacheService cacheService, IConfigurationQuerier configurationQuerier, IEventBus eventBus, EventContext eventContext,
    IEventSerializer eventSerializer) : base(eventBus, eventContext, eventSerializer)
  {
    _cacheService = cacheService;
    _configurationQuerier = configurationQuerier;
  }

  public async Task<ConfigurationAggregate?> LoadAsync(CancellationToken cancellationToken)
    => await LoadAsync(version: null, cancellationToken);
  public async Task<ConfigurationAggregate?> LoadAsync(long? version, CancellationToken cancellationToken)
  {
    AggregateId id = new ConfigurationId().AggregateId;
    return await LoadAsync<ConfigurationAggregate>(id, version, cancellationToken);
  }

  public async Task SaveAsync(ConfigurationAggregate configuration, CancellationToken cancellationToken)
  {
    await base.SaveAsync(configuration, cancellationToken);
    _cacheService.Configuration = await _configurationQuerier.ReadAsync(configuration, cancellationToken);
  }
}
