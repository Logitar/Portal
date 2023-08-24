using Logitar.EventSourcing;
using Logitar.Portal.Application.Actors;
using Logitar.Portal.Application.Configurations;
using Logitar.Portal.Contracts.Actors;
using Logitar.Portal.Contracts.Configurations;
using Logitar.Portal.Domain.Configurations;

namespace Logitar.Portal.EntityFrameworkCore.Relational.Queriers;

internal class ConfigurationQuerier : IConfigurationQuerier
{
  private readonly IActorService _actorService;

  public ConfigurationQuerier(IActorService actorService)
  {
    _actorService = actorService;
  }

  public async Task<Configuration> ReadAsync(ConfigurationAggregate configuration, CancellationToken cancellationToken)
  {
    HashSet<ActorId> actorIds = new(new[] { configuration.CreatedBy, configuration.UpdatedBy });
    Dictionary<ActorId, Actor> actors = await _actorService.FindAsync(actorIds, cancellationToken);
    Mapper mapper = new(actors);

    return mapper.ToConfiguration(configuration);
  }
}
