using Logitar.Portal.Contracts.Messages;
using Microsoft.Extensions.Caching.Memory;

namespace Logitar.Portal.MassTransitDemo.Worker.Caching;

internal class CacheService : ICacheService
{
  private readonly IMemoryCache _memoryCache;

  public CacheService(IMemoryCache memoryCache)
  {
    _memoryCache = memoryCache;
  }

  public SentMessages? GetSentMessages(Guid correlationId)
  {
    string key = GetSentMessagesKey(correlationId);
    return GetItem<SentMessages>(key);
  }
  public void SetSentMessages(Guid correlationId, SentMessages sentMessages)
  {
    string key = GetSentMessagesKey(correlationId);
    SetItem(key, sentMessages);
  }
  private static string GetSentMessagesKey(Guid correlationId) => $"SentMessages|CorrelationId:{correlationId}";

  private T? GetItem<T>(object key) => _memoryCache.TryGetValue(key, out object? value) ? (T?)value : default;
  private void SetItem<T>(object key, T value) => _memoryCache.Set(key, value);
}
