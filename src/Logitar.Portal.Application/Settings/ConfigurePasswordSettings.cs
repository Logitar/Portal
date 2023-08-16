using Logitar.Identity.Domain.Settings;
using Logitar.Portal.Domain;
using Logitar.Portal.Domain.Configurations;
using Microsoft.Extensions.Options;

namespace Logitar.Portal.Application.Settings;

internal class ConfigurePasswordSettings : IConfigureOptions<PasswordSettings>
{
  private readonly IApplicationContext _applicationContext;

  public ConfigurePasswordSettings(IApplicationContext applicationContext)
  {
    _applicationContext = applicationContext;
  }

  public void Configure(PasswordSettings settings)
  {
    // TODO(fpion): Realm
    ConfigurationAggregate configuration = _applicationContext.Configuration;

    ReadOnlyPasswordSettings passwordSettings = configuration.PasswordSettings;

    settings.RequiredLength = passwordSettings.RequiredLength;
    settings.RequiredUniqueChars = passwordSettings.RequiredUniqueChars;
    settings.RequireNonAlphanumeric = passwordSettings.RequireNonAlphanumeric;
    settings.RequireLowercase = passwordSettings.RequireLowercase;
    settings.RequireUppercase = passwordSettings.RequireUppercase;
    settings.RequireDigit = passwordSettings.RequireDigit;
    settings.Strategy = passwordSettings.Strategy;
  }
}
