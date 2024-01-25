using Logitar.EventSourcing.EntityFrameworkCore.Relational;
using Logitar.EventSourcing.Infrastructure;
using Logitar.Portal.Domain.Configurations;

namespace Logitar.Portal.EntityFrameworkCore.Relational.Repositories;

internal class ConfigurationRepository : EventSourcing.EntityFrameworkCore.Relational.AggregateRepository, IConfigurationRepository
{
  public ConfigurationRepository(IEventBus eventBus, EventContext eventContext, IEventSerializer eventSerializer)
    : base(eventBus, eventContext, eventSerializer)
  {
  }

  public async Task<ConfigurationAggregate?> LoadAsync(CancellationToken cancellationToken)
    => await LoadAsync<ConfigurationAggregate>(new ConfigurationId().AggregateId, cancellationToken);

  public async Task SaveAsync(ConfigurationAggregate configuration, CancellationToken cancellationToken)
    => await base.SaveAsync(configuration, cancellationToken); // TODO(fpion): saving the configuration should clear/update the cache?
}
