using Logitar.Identity.Contracts.Settings;
using Logitar.Identity.Domain.Settings;

namespace Logitar.Portal.Application.Settings;

internal class PortalUserSettingsResolver : IUserSettingsResolver
{
  private readonly IApplicationContext _applicationContext;

  public PortalUserSettingsResolver(IApplicationContext applicationContext)
  {
    _applicationContext = applicationContext;
  }

  public IUserSettings Resolve() => _applicationContext.UserSettings;
}
