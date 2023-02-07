using Logitar.Portal.Application;
using Logitar.Portal.Domain.Configurations;
using Microsoft.Extensions.Caching.Memory;

namespace Logitar.Portal.Infrastructure
{
  internal class CacheService : ICacheService
  {
    private const string ConfigurationKey = "Configuration";

    private readonly IMemoryCache _memoryCache;

    public CacheService(IMemoryCache memoryCache)
    {
      _memoryCache = memoryCache;
    }

    public Configuration? Configuration
    {
      get => GetItem<Configuration>(ConfigurationKey);
      set => SetItem(ConfigurationKey, value);
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
}
