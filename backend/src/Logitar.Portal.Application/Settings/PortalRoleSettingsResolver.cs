using Logitar.Identity.Contracts.Settings;
using Logitar.Identity.Domain.Settings;

namespace Logitar.Portal.Application.Settings;

internal class PortalRoleSettingsResolver : IRoleSettingsResolver
{
  private readonly IApplicationContext _applicationContext;

  public PortalRoleSettingsResolver(IApplicationContext applicationContext)
  {
    _applicationContext = applicationContext;
  }

  public IRoleSettings Resolve() => _applicationContext.RoleSettings;
}
