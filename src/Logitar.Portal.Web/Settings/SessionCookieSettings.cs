namespace Logitar.Portal.Web.Settings;

internal record SessionCookieSettings
{
  public SameSiteMode SameSite { get; set; } = SameSiteMode.Strict;
}
