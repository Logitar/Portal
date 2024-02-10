using Logitar.EventSourcing;
using Logitar.Identity.Contracts.Settings;
using Logitar.Identity.Domain.Settings;
using Logitar.Identity.Domain.Shared;
using Logitar.Portal.Application;
using Logitar.Portal.Application.Caching;
using Logitar.Portal.Contracts.Actors;
using Logitar.Portal.Contracts.ApiKeys;
using Logitar.Portal.Contracts.Configurations;
using Logitar.Portal.Contracts.Realms;
using Logitar.Portal.Contracts.Users;
using Logitar.Portal.Extensions;

namespace Logitar.Portal;

internal class HttpApplicationContext : IApplicationContext
{
  private readonly ICacheService _cacheService;
  private readonly IHttpContextAccessor _httpContextAccessor;
  private HttpContext Context => _httpContextAccessor.HttpContext
    ?? throw new InvalidOperationException($"The {nameof(_httpContextAccessor.HttpContext)} is required.");

  public Actor Actor
  {
    get
    {
      User? user = Context.GetUser();
      if (user != null)
      {
        return new Actor(user.FullName ?? user.UniqueName)
        {
          Id = user.Id,
          Type = ActorType.User,
          EmailAddress = user.Email?.Address,
          PictureUrl = user.Picture
        };
      }

      ApiKey? apiKey = Context.GetApiKey();
      if (apiKey != null)
      {
        return new Actor(apiKey.DisplayName)
        {
          Id = apiKey.Id,
          Type = ActorType.ApiKey
        };
      }

      return Actor.System;
    }
  }
  public ActorId ActorId => new(Actor.Id);

  public Uri BaseUri => Context.GetBaseUri();
  public string BaseUrl => BaseUri.ToString();

  public Configuration Configuration => _cacheService.Configuration
    ?? throw new InvalidOperationException("The configuration was not found in the cache.");
  public Realm? Realm => Context.GetRealm();
  public TenantId? TenantId => Realm == null ? null : new(new AggregateId(Realm.Id).Value);

  public IRoleSettings RoleSettings
  {
    get
    {
      Realm? realm = Realm;
      if (realm != null)
      {
        return new RoleSettings
        {
          UniqueName = realm.UniqueNameSettings
        };
      }

      Configuration configuration = Configuration;
      return new RoleSettings
      {
        UniqueName = configuration.UniqueNameSettings
      };
    }
  }
  public IUserSettings UserSettings
  {
    get
    {
      Realm? realm = Realm;
      if (realm != null)
      {
        return new UserSettings
        {
          UniqueName = realm.UniqueNameSettings,
          Password = realm.PasswordSettings,
          RequireUniqueEmail = realm.RequireUniqueEmail
        };
      }

      Configuration configuration = Configuration;
      return new UserSettings
      {
        UniqueName = configuration.UniqueNameSettings,
        Password = configuration.PasswordSettings,
        RequireUniqueEmail = configuration.RequireUniqueEmail
      };
    }
  }
  public HttpApplicationContext(ICacheService cacheService, IHttpContextAccessor httpContextAccessor)
  {
    _cacheService = cacheService;
    _httpContextAccessor = httpContextAccessor;
  }
}
