using Logitar.Portal.Contracts.Sessions.Models;
using Logitar.Portal.Contracts.Users.Models;
using Logitar.Portal.Web.Models.Users;
using System.Text;

namespace Logitar.Portal.Web.Extensions
{
  internal static class HttpContextExtensions
  {
    private const string SessionKey = "Session";
    private const string SessionIdKey = "SessionId";
    private const string UserKey = "User";

    public static CurrentUser GetCurrentUser(this HttpContext context) => new(context.GetUser());

    public static SessionModel? GetSession(this HttpContext context) => context.GetItem<SessionModel>(SessionKey);
    public static UserModel? GetUser(this HttpContext context) => context.GetItem<UserModel>(UserKey);
    private static T? GetItem<T>(this HttpContext context, object key)
    {
      if (context.Items.TryGetValue(key, out object? value))
      {
        return (T?)value;
      }

      return default;
    }

    public static bool SetSession(this HttpContext context, SessionModel? session) => context.SetItem(SessionKey, session);
    public static bool SetUser(this HttpContext context, UserModel? user) => context.SetItem(UserKey, user);
    private static bool SetItem(this HttpContext context, object key, object? value)
    {
      if (context.Items.ContainsKey(key))
      {
        if (!context.Items.Remove(key))
        {
          return false;
        }
      }

      return value != null && context.Items.TryAdd(key, value);
    }

    public static string? GetSessionId(this HttpContext context)
    {
      byte[]? bytes = context.Session.Get(SessionIdKey);

      return bytes == null ? null : Encoding.ASCII.GetString(bytes);
    }
    public static void SignIn(this HttpContext context, SessionModel session)
    {
      context.Session.Set(SessionIdKey, Encoding.ASCII.GetBytes(session.Id));

      if (session.RenewToken != null)
      {
        context.Response.Cookies.Append(Constants.Cookies.RenewToken, session.RenewToken, Constants.Cookies.RenewTokenOptions);
      }
    }
  }
}
