using Logitar.Portal.Application.Caching;
using Logitar.Portal.Domain.Configurations;
using Microsoft.Extensions.Caching.Memory;

namespace Logitar.Portal.Infrastructure.Caching;

internal class CacheService : ICacheService
{
  private const string ConfigurationKey = "Configuration";

  private readonly IMemoryCache _memoryCache;

  public CacheService(IMemoryCache memoryCache)
  {
    _memoryCache = memoryCache;
  }

  public ConfigurationAggregate? Configuration
  {
    get => _memoryCache.TryGetValue(ConfigurationKey, out ConfigurationAggregate? configuration) ? configuration : null;
    set
    {
      if (value == null)
      {
        _memoryCache.Remove(ConfigurationKey);
      }
      else
      {
        _memoryCache.Set(ConfigurationKey, value);
      }
    }
  }
}
