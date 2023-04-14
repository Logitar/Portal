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

  private T? GetItem<T>(object key) => _memoryCache.TryGetValue(key, out object? value) ? (T?)value : default;
  private void SetItem<T>(object key, T? value)
  {
    if (value == null)
    {
      _memoryCache.Remove(key);
    }
    else
    {
      _memoryCache.Set(key, value);
    }
  }
}
