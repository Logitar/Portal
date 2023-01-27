namespace Logitar.Portal.Web
{
  internal static class Constants
  {
    internal static class Cookies
    {
      public const string RenewToken = "renew_token";

      public static readonly CookieOptions RenewTokenOptions = new()
      {
        HttpOnly = true,
        MaxAge = TimeSpan.FromDays(7),
        SameSite = SameSiteMode.Strict,
        Secure = true
      };
    }

    public const string Version = "2.0";
  }
}
