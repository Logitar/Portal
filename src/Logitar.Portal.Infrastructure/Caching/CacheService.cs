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
  public void RemoveSession(SessionAggregate session)
  {
    string sessionKey = GetSessionCacheKey(session.Id.ToGuid());
    RemoveItem(sessionKey);

    string userSessionsKey = GetUserSessionsCacheKey(session.UserId.ToGuid());
    HashSet<string>? sessions = GetItem<HashSet<string>>(userSessionsKey);
    if (sessions != null)
    {
      sessions.Remove(sessionKey);

      if (sessions.Any())
      {
        SetItem(userSessionsKey, session, TimeSpan.FromMinutes(1));
      }
      else
      {
        RemoveItem(userSessionsKey);
      }
    }
  }
  public void SetSession(Session session)
  {
    TimeSpan expires = TimeSpan.FromMinutes(1);

    string sessionKey = GetSessionCacheKey(session.Id);
    SetItem(sessionKey, session, expires);

    string userSessionsKey = GetUserSessionsCacheKey(session.User.Id);
    HashSet<string> sessions = GetItem<HashSet<string>>(userSessionsKey) ?? new();
    sessions.Add(sessionKey);
    SetItem(userSessionsKey, sessions, expires);
  }
  private static string GetSessionCacheKey(Guid id) => string.Join(':', "Session", id);
  private static string GetUserSessionsCacheKey(Guid userId) => string.Join(':', "User", userId, "Sessions");

  public CachedUser? GetUser(string username) => GetItem<CachedUser>(GetUserCacheKey(username));
  public void RemoveUser(UserAggregate user)
  {
    string usernameKey = GetUsernameCacheKey(user);
    string? userKey = GetItem<string>(usernameKey);
    if (userKey != null)
    {
      RemoveItem(usernameKey);
      RemoveItem(userKey);
    }

    string userSessionsKey = GetUserSessionsCacheKey(user.Id.ToGuid());
    HashSet<string>? sessions = GetItem<HashSet<string>>(userSessionsKey);
    if (sessions != null)
    {
      foreach (string sessionKey in sessions)
      {
        RemoveItem(sessionKey);
      }

      RemoveItem(userSessionsKey);
    }
  }
  public void SetUser(string username, CachedUser user)
  {
    TimeSpan expires = TimeSpan.FromMinutes(1);
    object userKey = GetUserCacheKey(username);
    SetItem(userKey, user, expires);
    SetItem(GetUsernameCacheKey(user.Aggregate), userKey, expires);
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
