using Logitar.Identity.Contracts.Settings;
using Logitar.Identity.Domain.Settings;
using Logitar.Portal.Contracts.Realms;
using Logitar.Portal.Domain.Configurations;

namespace Logitar.Portal.Application.Settings;

public static class SettingsExtensions
{
  public static IRoleSettings GetRoleSettings(this ConfigurationAggregate configuration) => new RoleSettings
  {
    UniqueName = configuration.UniqueNameSettings
  };
  public static IRoleSettings GetRoleSettings(this Realm realm) => new RoleSettings
  {
    UniqueName = realm.UniqueNameSettings
  };

  public static IUserSettings GetUserSettings(this ConfigurationAggregate configuration) => new UserSettings
  {
    UniqueName = configuration.UniqueNameSettings,
    Password = configuration.PasswordSettings,
    RequireUniqueEmail = configuration.RequireUniqueEmail
  };
  public static IUserSettings GetUserSettings(this Realm realm) => new UserSettings
  {
    UniqueName = realm.UniqueNameSettings,
    Password = realm.PasswordSettings,
    RequireUniqueEmail = realm.RequireUniqueEmail
  };
}
