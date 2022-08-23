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

    internal static class Headers
    {
      public const string ApiKey = "X-API-Key";
      public const string Session = "X-Session";
    }

    internal static class Policies
    {
      public const string ApiKey = nameof(ApiKey);
      public const string AuthenticatedUser = nameof(AuthenticatedUser);
      public const string PortalIdentity = nameof(PortalIdentity);
      public const string Session = nameof(Session);
    }

    internal static class Schemes
    {
      public const string ApiKey = nameof(ApiKey);
      public const string Session = nameof(Session);

      public static string[] All => new[] { ApiKey, Session };
    }

    public const string Version = "1.1.2";
  }
}
