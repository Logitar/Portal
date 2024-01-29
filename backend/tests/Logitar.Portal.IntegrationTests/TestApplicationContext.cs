using Logitar.EventSourcing;
using Logitar.Identity.Contracts.Settings;
using Logitar.Identity.Domain.Settings;
using Logitar.Portal.Application;
using Logitar.Portal.Application.Caching;
using Logitar.Portal.Contracts.Actors;
using Logitar.Portal.Contracts.Configurations;
using Logitar.Portal.Contracts.Realms;

namespace Logitar.Portal;

public class TestApplicationContext : IApplicationContext
{
  private readonly ICacheService _cacheService;

  public Actor Actor => throw new NotImplementedException();
  public ActorId ActorId => throw new NotImplementedException();

  public Configuration Configuration => _cacheService.Configuration
    ?? throw new InvalidOperationException("The configuration was not found in the cache.");
  public Realm? Realm { get; set; }

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

  public TestApplicationContext(ICacheService cacheService)
  {
    _cacheService = cacheService;
  }
}
