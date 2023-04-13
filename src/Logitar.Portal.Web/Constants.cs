namespace Logitar.Portal.Web;

internal static class Constants
{
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

  internal static class Headers
  {
    public const string Authorization = nameof(Authorization);
  }

  internal static class Policies
  {
    public const string AuthenticatedPortalUser = nameof(AuthenticatedPortalUser);
    public const string PortalActor = nameof(PortalActor);
  }

  internal static class Schemes
  {
    public const string Basic = nameof(Basic);
    public const string Session = nameof(Session);

    public static string[] All => new[] { Basic, Session };
  }

  public static readonly Version Version = new(2, 0, 0);
}
