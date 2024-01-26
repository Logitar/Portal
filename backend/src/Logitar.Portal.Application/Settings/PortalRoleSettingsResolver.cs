using Logitar.Identity.Contracts.Settings;
using Logitar.Identity.Domain.Settings;
using Microsoft.Extensions.Configuration;

namespace Logitar.Portal.Application.Settings;

internal class PortalRoleSettingsResolver : RoleSettingsResolver
{
  private readonly IApplicationContext _context;

  public PortalRoleSettingsResolver(IConfiguration configuration, IApplicationContext context) : base(configuration)
  {
    _context = context;
  }

  public override IRoleSettings Resolve() => _context.RoleSettings;
}
