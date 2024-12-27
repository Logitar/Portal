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

  public ConfigurationQuerier(IActorService actorService)
  {
    _actorService = actorService;
  }

  public async Task<ConfigurationModel> ReadAsync(Configuration configuration, CancellationToken cancellationToken)
  {
    List<ActorId> actorIds = new(capacity: 2);
    if (configuration.CreatedBy.HasValue)
    {
      actorIds.Add(configuration.CreatedBy.Value);
    }
    if (configuration.UpdatedBy.HasValue)
    {
      actorIds.Add(configuration.UpdatedBy.Value);
    }
    IEnumerable<ActorModel> actors = await _actorService.FindAsync(actorIds, cancellationToken);
    Mapper mapper = new(actors);

    return mapper.ToConfiguration(configuration);
  }
}
