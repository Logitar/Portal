using Logitar.Portal.Contracts.Actors;
using Logitar.Portal.Core.Caching;
using Logitar.Portal.Core.Realms;
using Microsoft.Extensions.Caching.Memory;

namespace Logitar.Portal.Infrastructure.Caching;

internal class CacheService : ICacheService
{
  private const string PortalRealmKey = nameof(PortalRealm);

  private readonly IMemoryCache _memoryCache;

  public CacheService(IMemoryCache memoryCache)
  {
    _memoryCache = memoryCache;
  }

  public RealmAggregate? PortalRealm
  {
    get => GetItem<RealmAggregate>(PortalRealmKey);
    set => SetItem(PortalRealmKey, value);
  }

  public Actor? GetActor(Guid id) => GetItem<Actor>(GetActorCacheKey(id));
  public void RemoveActor(Guid id) => RemoveItem(GetActorCacheKey(id));
  public void SetActor(Actor actor) => SetItem(GetActorCacheKey(actor.Id), actor, TimeSpan.FromMinutes(10));
  private string GetActorCacheKey(Guid id) => string.Join('_', nameof(Actor), id);

  private T? GetItem<T>(object key) => _memoryCache.TryGetValue(key, out object? value) ? (T?)value : default;
  private void RemoveItem(object key) => _memoryCache.Remove(key);
  private void SetItem<T>(object key, T? value, TimeSpan? expires = null)
  {
    if (value == null)
    {
      RemoveItem(key);
    }
    else
    {
      if (expires.HasValue)
      {
        _memoryCache.Set(key, value, expires.Value);
      }
      else
      {
        _memoryCache.Set(key, value);
      }
    }
  }
}
