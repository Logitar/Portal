using System.Globalization;

namespace Logitar.Portal.v2.Core;

public static class Constants
{
  public static class PortalRealm
  {
    public const string UniqueName = "portal";
    public const string DisplayName = "Portal";
    public const string Description = "The realm in which the administrator users of the Portal belong to.";

    public static readonly CultureInfo DefaultLocale = CultureInfo.GetCultureInfo("en");
  }
}
