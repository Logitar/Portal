﻿namespace Logitar.Portal.Web.Settings;

public record CookiesSettings
{
  public const string SectionKey = "Cookies";

  public RefreshTokenCookieSettings RefreshToken { get; set; } = new();
  public SessionCookieSettings Session { get; set; } = new();
}
