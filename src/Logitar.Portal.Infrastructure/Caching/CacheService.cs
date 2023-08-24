using Logitar.EventSourcing;
using Logitar.Portal.Application.Caching;
using Logitar.Portal.Contracts.Actors;
using Logitar.Portal.Domain.Configurations;
using Microsoft.Extensions.Caching.Memory;

namespace Logitar.Portal.Infrastructure.Caching;

internal class CacheService : ICacheService
{
  private const string ConfigurationKey = "Configuration";

  private readonly IMemoryCache _cache;

  public CacheService(IMemoryCache cache)
  {
    _cache = cache;
  }

  public ConfigurationAggregate? Configuration
  {
    get => GetItem<ConfigurationAggregate>(ConfigurationKey);
    set => SetItem(ConfigurationKey, value);
  }

  public Actor? GetActor(ActorId id) => GetItem<Actor>(GetActorKey(id));
  public void RemoveActor(ActorId id) => SetItem<Actor>(GetActorKey(id), value: null);
  public void SetActor(ActorId id, Actor actor) => SetItem(GetActorKey(id), actor);
  private static string GetActorKey(ActorId id) => $"Actor_{id}";

  private T? GetItem<T>(object key) => _cache.TryGetValue(key, out T? value) ? value : default;
  private void SetItem<T>(object key, T? value)
  {
    if (value == null)
    {
      _cache.Remove(key);
    }
    else
    {
      _cache.Set(key, value);
    }
  }
}
