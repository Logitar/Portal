using Logitar.Portal.Contracts.Actors;
using Logitar.Portal.Contracts.Sessions;
using Logitar.Portal.Core.Caching;
using Logitar.Portal.Core.Realms;
using Logitar.Portal.Core.Sessions;
using Logitar.Portal.Core.Users;
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
  private static string GetActorCacheKey(Guid id) => string.Join(':', nameof(Actor), id);

  public Session? GetSession(Guid id) => GetItem<Session>(GetSessionCacheKey(id));
  public void RemoveSession(SessionAggregate session) => RemoveItem(GetSessionCacheKey(session.Id.ToGuid()));
  public void SetSession(Session session) => SetItem(GetSessionCacheKey(session.Id), session, TimeSpan.FromMinutes(1));
  private static string GetSessionCacheKey(Guid id) => string.Join(':', "Session", id);

  public CachedUser? GetUser(string username) => GetItem<CachedUser>(GetUserCacheKey(username));
  public void RemoveUser(UserAggregate user)
  {
    string key = GetUsernameCacheKey(user);
    string? usernameKey = GetItem<string>(key);
    if (usernameKey != null)
    {
      RemoveItem(key);
      RemoveItem(usernameKey);
    }
  }
  public void SetUser(string username, CachedUser user)
  {
    TimeSpan expires = TimeSpan.FromMinutes(1);
    object key = GetUserCacheKey(username);
    SetItem(key, user, expires);
    SetItem(GetUsernameCacheKey(user.Aggregate), key, expires);
  }
  private static string GetUserCacheKey(string username) => string.Join(':', "User", "Username", username.Trim().ToUpper());
  private static string GetUsernameCacheKey(UserAggregate user) => string.Join(':', "Username", user.Id);

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
