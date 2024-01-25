using Logitar.Identity.Contracts.Settings;
using Logitar.Identity.Domain.Settings;
using Logitar.Portal.Contracts.Realms;
using Microsoft.Extensions.Configuration;

namespace Logitar.Portal.Application.Settings;

internal class PortalUserSettingsResolver : UserSettingsResolver
{
  private readonly IApplicationContext _context;

  public PortalUserSettingsResolver(IConfiguration configuration, IApplicationContext context) : base(configuration)
  {
    _context = context;
  }

  public override IUserSettings Resolve()
  {
    Realm realm = _context.Realm;
    return new UserSettings
    {
      UniqueName = realm.UniqueNameSettings,
      Password = realm.PasswordSettings,
      RequireUniqueEmail = realm.RequireUniqueEmail
    };
  }
}
