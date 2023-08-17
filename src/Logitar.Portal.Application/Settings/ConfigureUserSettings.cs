using Logitar.Identity.Domain.Settings;
using Logitar.Portal.Domain;
using Logitar.Portal.Domain.Configurations;
using Logitar.Portal.Domain.Realms;
using Microsoft.Extensions.Options;

namespace Logitar.Portal.Application.Settings;

internal class ConfigureUserSettings : IConfigureOptions<UserSettings>
{
  private readonly IApplicationContext _applicationContext;
  private readonly IConfigureOptions<PasswordSettings> _configurePasswordSettings;

  public ConfigureUserSettings(IApplicationContext applicationContext,
    IConfigureOptions<PasswordSettings> configurePasswordSettings)
  {
    _applicationContext = applicationContext;
    _configurePasswordSettings = configurePasswordSettings;
  }

  public void Configure(UserSettings settings)
  {
    RealmAggregate? realm = _applicationContext.Realm;
    ConfigurationAggregate configuration = _applicationContext.Configuration;

    ReadOnlyUniqueNameSettings _uniqueNameSettings = realm?.UniqueNameSettings ?? configuration.UniqueNameSettings;
    UniqueNameSettings uniqueNameSettings = new()
    {
      AllowedCharacters = _uniqueNameSettings.AllowedCharacters
    };

    PasswordSettings passwordSettings = new();
    _configurePasswordSettings.Configure(passwordSettings);

    settings.RequireUniqueEmail = realm?.RequireUniqueEmail ?? false;
    settings.RequireConfirmedAccount = realm?.RequireConfirmedAccount ?? false;

    settings.UniqueNameSettings = uniqueNameSettings;
    settings.PasswordSettings = passwordSettings;
  }
}
