using Logitar.EventSourcing;
using Logitar.Portal.Application.Caching;
using Logitar.Portal.Contracts.Actors;
using Logitar.Portal.Contracts.Sessions;
using Logitar.Portal.Domain.Configurations;
using Logitar.Portal.Domain.Sessions;
using Logitar.Portal.Domain.Users;
using Microsoft.Extensions.Caching.Memory;

namespace Logitar.Portal.Infrastructure.Caching;

internal class CacheService : ICacheService
{
  private const string ConfigurationKey = "Configuration";

  private static readonly TimeSpan _actorExpiration = TimeSpan.FromHours(1);
  private static readonly TimeSpan _authenticationExpiration = TimeSpan.FromMinutes(5);

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
  public void SetActor(ActorId id, Actor actor) => SetItem(GetActorKey(id), actor, _actorExpiration);
  private static string GetActorKey(ActorId id) => $"Actor:Id:{id}";

  public Session? GetSession(Guid id) => GetItem<Session>(GetSessionKey(id));
  public void RemoveSession(SessionAggregate session)
  {
    Guid sessionId = session.Id.ToGuid();
    RemoveItem(GetSessionKey(sessionId));

    string userSessionsKey = GetUserSessionsKey(session.UserId.ToGuid());
    HashSet<Guid>? userSessions = GetItem<HashSet<Guid>>(userSessionsKey);
    if (userSessions != null)
    {
      userSessions.Remove(sessionId);

      if (userSessions.Any())
      {
        SetItem(userSessionsKey, userSessions, _authenticationExpiration);
      }
      else
      {
        RemoveItem(userSessionsKey);
      }
    }
  }
  public void SetSession(Session session)
  {
    SetItem(GetSessionKey(session.Id), session, _authenticationExpiration);

    string userSessionsKey = GetUserSessionsKey(session.User.Id);
    HashSet<Guid> userSessions = GetItem<HashSet<Guid>>(userSessionsKey) ?? new();
    userSessions.Add(session.Id);
    SetItem(userSessionsKey, userSessions, _authenticationExpiration);
  }
  private static string GetSessionKey(Guid id) => $"Session:Id:{id}";
  private static string GetUserSessionsKey(Guid id) => $"User:Id:{id}:Sessions";

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

    string userSessionsKey = GetUserSessionsKey(user.Id.ToGuid());
    HashSet<Guid>? userSessions = GetItem<HashSet<Guid>>(userSessionsKey);
    if (userSessions != null)
    {
      RemoveItem(userSessionsKey);

      foreach (Guid sessionId in userSessions)
      {
        string sessionKey = GetSessionKey(sessionId);
        RemoveItem(sessionKey);
      }
    }

    ActorId actorId = new(user.Id.Value);
    RemoveActor(actorId);
  }
  public void SetUser(CachedUser user)
  {
    string userKey = GetUserKey(user.Aggregate);
    SetItem(userKey, user, _authenticationExpiration);

    string uniqueNameKey = GetUniqueNameKey(user.Aggregate.UniqueName);
    SetItem(uniqueNameKey, userKey, _authenticationExpiration);
  }
  private static string GetUniqueNameKey(string uniqueName) => $"User:UniqueName:{uniqueName.Trim().ToUpper()}";
  private static string GetUserKey(UserAggregate user) => $"User:Id:{user.Id}";

  private T? GetItem<T>(object key) => _cache.TryGetValue(key, out T? value) ? value : default;
  private void RemoveItem(object key) => _cache.Remove(key);
  private void SetItem<T>(object key, T value) => _cache.Set(key, value);
  private void SetItem<T>(object key, T value, TimeSpan expiration) => _cache.Set(key, value, expiration);
}
