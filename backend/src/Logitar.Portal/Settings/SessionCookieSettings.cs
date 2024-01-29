namespace Logitar.Portal.Settings;

internal record SessionCookieSettings
{
  public SameSiteMode SameSite { get; set; } = SameSiteMode.Strict;
}
