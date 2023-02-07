using Logitar.Portal.Application;
using Logitar.Portal.Contracts.Sessions;
using Logitar.Portal.Domain;
using Logitar.Portal.Domain.Configurations;
using Microsoft.Extensions.Caching.Memory;

namespace Logitar.Portal.Infrastructure
{
  internal class CacheService : ICacheService
  {
    private const string ApiKeyKeyFormat = "ApiKey_{0}";
    private const string ConfigurationKey = "Configuration";
    private const string SessionKeyFormat = "Session_{0}";

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

    public CachedApiKey? GetApiKey(AggregateId id) => GetItem<CachedApiKey>(string.Format(ApiKeyKeyFormat, id));
    public void RemoveApiKey(AggregateId id) => RemoveItem(string.Format(ApiKeyKeyFormat, id));
    public void SetApiKey(CachedApiKey apiKey) => SetItem(string.Format(ApiKeyKeyFormat, apiKey.Id), apiKey, TimeSpan.FromHours(1));

    public SessionModel? GetSession(string id) => GetItem<SessionModel>(string.Format(SessionKeyFormat, id));
    public void RemoveSession(AggregateId id) => RemoveItem(string.Format(SessionKeyFormat, id));
    public void RemoveSessions(IEnumerable<string> ids)
    {
      foreach (string id in ids)
      {
        RemoveItem(string.Format(SessionKeyFormat, id));
      }
    }
    public void SetSession(SessionModel session) => SetItem(string.Format(SessionKeyFormat, session.Id), session, TimeSpan.FromMinutes(10));

    private T? GetItem<T>(object key) => _memoryCache.TryGetValue(key, out object? value) ? (T?)value : default;
    private void RemoveItem(object key) => _memoryCache.Remove(key);
    private void SetItem<T>(object key, T value)
    {
      if (value == null)
      {
        RemoveItem(key);
      }
      else
      {
        _memoryCache.Set(key, value);
      }
    }
    private void SetItem<T>(object key, T value, TimeSpan expiration)
    {
      if (value == null)
      {
        RemoveItem(key);
      }
      else
      {
        _memoryCache.Set(key, value, expiration);
      }
    }
  }
}
