using Logitar.EventSourcing;
using Logitar.Portal.Application.Caching;
using Logitar.Portal.Contracts.Actors;
using Microsoft.Extensions.Caching.Memory;

namespace Logitar.Portal.Infrastructure.Caching;

internal class CacheService : ICacheService
{
  private readonly IMemoryCache _memoryCache;

  public CacheService(IMemoryCache memoryCache)
  {
    _memoryCache = memoryCache;
  }

  public Actor? GetActor(ActorId id) => _memoryCache.TryGetValue(GetActorKey(id), out Actor? actor) ? actor : null;
  public void RemoveActor(ActorId id) => _memoryCache.Remove(GetActorKey(id));
  public void SetActor(ActorId id, Actor actor) => _memoryCache.Set(GetActorKey(id), actor, TimeSpan.FromMinutes(10));
  private static string GetActorKey(ActorId id) => $"Actor_{id}";
}
