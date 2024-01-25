using Logitar.Identity.Contracts.Settings;
using Logitar.Identity.Domain.Settings;
using Logitar.Portal.Contracts.Realms;
using Microsoft.Extensions.Configuration;

namespace Logitar.Portal.Application.Settings;

internal class PortalRoleSettingsResolver : RoleSettingsResolver
{
  private readonly IApplicationContext _context;

  public PortalRoleSettingsResolver(IConfiguration configuration, IApplicationContext context) : base(configuration)
  {
    _context = context;
  }

  public override IRoleSettings Resolve()
  {
    Realm realm = _context.Realm;
    return new RoleSettings
    {
      UniqueName = realm.UniqueNameSettings
    };
  }
}
