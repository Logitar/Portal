using Logitar.EventSourcing;
using Logitar.Portal.Application.Caching;
using Logitar.Portal.Contracts.Actors;
using Logitar.Portal.Domain.Configurations;
using Logitar.Portal.Domain.Users;
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
  public void RemoveActor(ActorId id) => RemoveItem(GetActorKey(id));
  public void SetActor(ActorId id, Actor actor) => SetItem(GetActorKey(id), actor);
  private static string GetActorKey(ActorId id) => $"Actor:Id:{id}";

  public CachedUser? GetUser(string uniqueName)
  {
    string uniqueNameKey = GetUniqueNameKey(uniqueName);
    string? userKey = GetItem<string>(uniqueNameKey);

    return userKey == null ? null : GetItem<CachedUser>(userKey);
  }
  public void RemoveUser(UserAggregate user)
  {
    string userKey = GetUserKey(user);

    CachedUser? cached = GetItem<CachedUser>(userKey);
    string uniqueNameKey = GetUniqueNameKey(cached == null ? user.UniqueName : cached.Aggregate.UniqueName);

    RemoveItem(uniqueNameKey);
    RemoveItem(userKey);
  }
  public void SetUser(CachedUser user)
  {
    string userKey = GetUserKey(user.Aggregate);
    SetItem(userKey, user);

    string uniqueNameKey = GetUniqueNameKey(user.Aggregate.UniqueName);
    SetItem(uniqueNameKey, userKey);
  }
  private static string GetUniqueNameKey(string uniqueName) => $"User:UniqueName:{uniqueName.Trim().ToUpper()}";
  private static string GetUserKey(UserAggregate user) => $"User:Id:{user.Id}";

  private T? GetItem<T>(object key) => _cache.TryGetValue(key, out T? value) ? value : default;
  private void RemoveItem(object key) => _cache.Remove(key);
  private void SetItem<T>(object key, T value) => _cache.Set(key, value);
}
