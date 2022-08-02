using Logitar.Portal.Core.ApiKeys;
using Logitar.Portal.Core.Sessions;
using Logitar.Portal.Core.Sessions.Models;
using Logitar.Portal.Core.Users;

namespace Logitar.Portal.Web
{
  internal static class HttpContextExtensions
  {
    private const string ApiKeyKey = nameof(ApiKey);
    private const string SessionIdKey = "SessionId";
    private const string SessionKey = nameof(Session);
    private const string UserKey = nameof(User);

    public static ApiKey? GetApiKey(this HttpContext context) => context.GetItem<ApiKey>(ApiKeyKey);
    public static Session? GetSession(this HttpContext context) => context.GetItem<Session>(SessionKey);
    public static User? GetUser(this HttpContext context) => context.GetItem<User>(UserKey);
    private static T? GetItem<T>(this HttpContext context, object key)
    {
      if (context.Items.TryGetValue(key, out object? value))
      {
        return (T?)value;
      }

      return default;
    }

    public static bool SetApiKey(this HttpContext context, ApiKey? apiKey) => context.SetItem(ApiKeyKey, apiKey);
    public static bool SetSession(this HttpContext context, Session? session) => context.SetItem(SessionKey, session);
    public static bool SetUser(this HttpContext context, User? user) => context.SetItem(UserKey, user);
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

    public static Guid? GetSessionId(this HttpContext context)
    {
      byte[]? bytes = context.Session.Get(SessionIdKey);

      return bytes == null ? null : new Guid(bytes);
    }
    public static void SetSession(this HttpContext context, SessionModel session)
    {
      ArgumentNullException.ThrowIfNull(context);
      ArgumentNullException.ThrowIfNull(session);

      context.Session.Set(SessionIdKey, session.Id.ToByteArray());

      if (session.RenewToken != null)
      {
        context.Response.Cookies.Append(Constants.Cookies.RenewToken, session.RenewToken, Constants.Cookies.RenewTokenOptions);
      }
    }
  }
}
