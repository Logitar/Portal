using Logitar.Portal.Contracts.Sessions;
using Logitar.Portal.Web.Constants;

namespace Logitar.Portal.Web.Extensions;

internal static class HttpContextExtensions
{
  private const string SessionIdKey = "SessionId";

  public static Guid? GetSessionId(this HttpContext context)
  {
    byte[]? bytes = context.Session.Get(SessionIdKey);

    return bytes == null ? null : new(bytes);
  }
  public static bool IsSignedIn(this HttpContext context) => context.GetSessionId().HasValue;
  public static void SignIn(this HttpContext context, Session session)
  {
    context.Session.Set(SessionIdKey, session.Id.ToByteArray());

    if (session.RefreshToken != null)
    {
      context.Response.Cookies.Append(Cookies.RefreshToken, session.RefreshToken, Cookies.RefreshTokenOptions);
    }
  }
  public static void SignOut(this HttpContext context)
  {
    context.Session.Clear();

    context.Response.Cookies.Delete(Cookies.RefreshToken);
  }
}
