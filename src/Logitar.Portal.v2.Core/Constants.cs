using System.Globalization;

namespace Logitar.Portal.v2.Core;

public static class Constants
{
  public static class PortalRealm
  {
    public const string UniqueName = "portal";
    public const string DisplayName = "Portal";
    public const string Description = ""; // TODO(fpion): implement

    public static readonly CultureInfo DefaultLocale = CultureInfo.GetCultureInfo("en");
  }

  public static readonly Version Version = new(2, 0, 0);
}
