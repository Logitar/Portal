using Logitar.EventSourcing;
using Logitar.Portal.Application.Actors;
using Logitar.Portal.Application.Caching;
using Logitar.Portal.Contracts.Actors;
using Logitar.Portal.EntityFrameworkCore.Relational.Entities;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Portal.EntityFrameworkCore.Relational.Actors;

internal class ActorService : IActorService
{
  private readonly DbSet<ActorEntity> _actors;
  private readonly ICacheService _cacheService;

  public ActorService(ICacheService cacheService, PortalContext context)
  {
    _actors = context.Actors;
    _cacheService = cacheService;
  }

  public async Task<Dictionary<ActorId, Actor>> FindAsync(IEnumerable<ActorId> ids, CancellationToken cancellationToken)
  {
    Dictionary<ActorId, Actor> actors = new(ids.Count());

    List<Guid> missingActors = new(capacity: actors.Count);
    foreach (ActorId id in ids)
    {
      Actor? actor = _cacheService.GetActor(id);
      if (actor == null)
      {
        missingActors.Add(id.ToGuid());
      }
      else
      {
        actors[id] = actor;
        _cacheService.SetActor(id, actor);
      }
    }

    if (missingActors.Any())
    {
      ActorEntity[] entities = await _actors.AsNoTracking()
        .Where(x => missingActors.Contains(x.Id))
        .ToArrayAsync(cancellationToken);

      foreach (ActorEntity entity in entities)
      {
        ActorId id = new(entity.Id);
        Actor actor = entity.ToActor();

        actors[id] = actor;
        _cacheService.SetActor(id, actor);
      }
    }

    return actors;
  }
}
