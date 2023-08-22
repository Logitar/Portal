namespace Logitar.Portal.Constants;

internal static class Cookies
{
  public const string RefreshToken = "refresh_token";

  public static readonly CookieOptions RefreshTokenOptions = new()
  {
    HttpOnly = true,
    MaxAge = TimeSpan.FromDays(7),
    SameSite = SameSiteMode.Strict,
    Secure = true
  };
}
