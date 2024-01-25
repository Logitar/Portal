using Logitar.EventSourcing;
using Logitar.Portal.Application.Caching;
using Logitar.Portal.Contracts.Actors;
using Logitar.Portal.Contracts.Configurations;
using Logitar.Portal.Domain.Configurations;
using Logitar.Portal.Infrastructure.Settings;
using Microsoft.Extensions.Caching.Memory;

namespace Logitar.Portal.Infrastructure.Caching;

internal class CacheService : ICacheService
{
  private readonly IMemoryCache _memoryCache;
  private readonly CachingSettings _settings;

  public CacheService(IMemoryCache memoryCache, CachingSettings settings)
  {
    _memoryCache = memoryCache;
    _settings = settings;
  }

  public Actor? GetActor(ActorId id) => GetItem<Actor>(GetActorKey(id));
  public void SetActor(Actor actor) => SetItem(GetActorKey(actor.Id), actor, _settings.ActorLifetime);
  public void RemoveActor(ActorId id) => RemoveItem(GetActorKey(id));
  private static string GetActorKey(string id) => GetActorKey(new ActorId(id));
  private static string GetActorKey(ActorId id) => $"Actor.Id:{id}";

  public Configuration? GetConfiguration() => GetItem<Configuration>(GetConfigurationKey());
  public void SetConfiguration(Configuration configuration) => SetItem(GetConfigurationKey(), configuration);
  private static string GetConfigurationKey() => new ConfigurationId().Value;

  private T? GetItem<T>(object key) => _memoryCache.TryGetValue(key, out object? value) ? (T?)value : default;
  private void RemoveItem(object key) => _memoryCache.Remove(key);
  private void SetItem<T>(object key, T value, TimeSpan? lifetime = null)
  {
    if (lifetime.HasValue)
    {
      _memoryCache.Set(key, value, lifetime.Value);
    }
    else
    {
      _memoryCache.Set(key, value);
    }
  }
}
