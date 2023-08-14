using Logitar.EventSourcing.EntityFrameworkCore.Relational;
using Logitar.EventSourcing.Infrastructure;
using Logitar.Portal.Domain.Configurations;

namespace Logitar.Portal.EntityFrameworkCore.Relational.Repositories;

internal class ConfigurationRepository : EventSourcing.EntityFrameworkCore.Relational.AggregateRepository, IConfigurationRepository
{
  public ConfigurationRepository(IEventBus eventBus, EventContext eventContext,
    IEventSerializer eventSerializer) : base(eventBus, eventContext, eventSerializer)
  {
  }

  public async Task<ConfigurationAggregate?> LoadAsync(CancellationToken cancellationToken)
    => (await base.LoadAsync<ConfigurationAggregate>(cancellationToken)).SingleOrDefault();

  public async Task SaveAsync(ConfigurationAggregate configuration, CancellationToken cancellationToken)
    => await base.SaveAsync(configuration, cancellationToken);
}
