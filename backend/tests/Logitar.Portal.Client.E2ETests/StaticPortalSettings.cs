using Logitar.Portal.Client;

namespace Logitar.Portal;

internal record StaticPortalSettings : PortalSettings
{
  private static StaticPortalSettings? _instance = null;
  public static StaticPortalSettings Instance
  {
    get
    {
      _instance ??= new();
      return _instance;
    }
  }

  private StaticPortalSettings()
  {
  }
}
