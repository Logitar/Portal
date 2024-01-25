using Logitar.EventSourcing;
using Logitar.Portal.Application.Configurations;
using Logitar.Portal.Contracts.Actors;
using Logitar.Portal.Contracts.Configurations;
using Logitar.Portal.Domain.Configurations;
using Logitar.Portal.EntityFrameworkCore.Relational.Actors;

namespace Logitar.Portal.EntityFrameworkCore.Relational.Queriers;

internal class ConfigurationQuerier : IConfigurationQuerier
{
  private readonly IActorService _actorService;
  private readonly IConfigurationRepository _configurationRepository;

  public ConfigurationQuerier(IActorService actorService, IConfigurationRepository configurationRepository)
  {
    _actorService = actorService;
    _configurationRepository = configurationRepository;
  }

  public async Task<Configuration?> ReadAsync(CancellationToken cancellationToken)
  {
    ConfigurationAggregate? configuration = await _configurationRepository.LoadAsync(cancellationToken);
    return configuration == null ? null : await ReadAsync(configuration, cancellationToken);
  }

  public async Task<Configuration> ReadAsync(ConfigurationAggregate configuration, CancellationToken cancellationToken)
  {
    IEnumerable<ActorId> actorIds = [configuration.CreatedBy, configuration.UpdatedBy];
    IEnumerable<Actor> actors = await _actorService.FindAsync(actorIds, cancellationToken);
    Mapper mapper = new(actors);

    return mapper.ToConfiguration(configuration);
  }
}
