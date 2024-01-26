using Logitar.Identity.Contracts.Settings;
using Logitar.Identity.Domain.Settings;
using Microsoft.Extensions.Configuration;

namespace Logitar.Portal.Application.Settings;

internal class PortalUserSettingsResolver : UserSettingsResolver
{
  private readonly IApplicationContext _context;

  public PortalUserSettingsResolver(IConfiguration configuration, IApplicationContext context) : base(configuration)
  {
    _context = context;
  }

  public override IUserSettings Resolve() => _context.UserSettings;
}
