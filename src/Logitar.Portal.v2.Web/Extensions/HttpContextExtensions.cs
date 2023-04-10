using Logitar.Portal.v2.Contracts.Sessions;
using Logitar.Portal.v2.Contracts.Users;
using Logitar.Portal.v2.Web.Models;

namespace Logitar.Portal.v2.Web.Extensions;

internal static class HttpContextExtensions
{
  private const string SessionIdKey = "SessionId";
  private const string UserKey = "User";

  public static CurrentUser GetCurrentUser(this HttpContext context) => new(context.GetUser());

  public static User? GetUser(this HttpContext context) => context.GetItem<User>(UserKey);

  private static T? GetItem<T>(this HttpContext context, object key)
  {
    return context.Items.TryGetValue(key, out object? value) ? (T?)value : default;
  }

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
      context.Response.Cookies.Append(WebConstants.Cookies.RefreshToken, session.RefreshToken, WebConstants.Cookies.RefreshTokenOptions);
    }
  }
}
