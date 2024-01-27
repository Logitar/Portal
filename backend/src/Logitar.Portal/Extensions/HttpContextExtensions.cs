using Logitar.Portal.Constants;
using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.ApiKeys;
using Logitar.Portal.Contracts.Realms;
using Logitar.Portal.Contracts.Sessions;
using Logitar.Portal.Contracts.Users;
using Logitar.Portal.Settings;
using Microsoft.Extensions.Primitives;

namespace Logitar.Portal.Extensions;

internal static class HttpContextExtensions
{
  private const string ApiKeyKey = nameof(ApiKey);
  private const string RealmKey = nameof(Realm);
  private const string SessionKey = nameof(Session);
  private const string SessionIdKey = "SessionId";
  private const string UserKey = nameof(User);

  public static IEnumerable<CustomAttribute> GetSessionCustomAttributes(this HttpContext context)
  {
    List<CustomAttribute> customAttributes = new(capacity: 2)
    {
      new("AdditionalInformation", context.GetAdditionalInformation())
    };

    string? ipAddress = context.GetClientIpAddress();
    if (ipAddress != null)
    {
      customAttributes.Add(new("IpAddress", ipAddress));
    }

    return customAttributes;
  }
  private static string GetAdditionalInformation(this HttpContext context)
  {
    return JsonSerializer.Serialize(context.Request.Headers);
  }
  private static string? GetClientIpAddress(this HttpContext context)
  {
    string? ipAddress = null;

    if (context.Request.Headers.TryGetValue("X-Forwarded-For", out StringValues xForwardedFor))
    {
      ipAddress = xForwardedFor.Single()?.Split(':').First();
    }
    ipAddress ??= context.Connection.RemoteIpAddress?.ToString();

    return ipAddress;
  }

  public static ApiKey? GetApiKey(this HttpContext context) => context.GetItem<ApiKey>(ApiKeyKey);
  public static Realm? GetRealm(this HttpContext context) => context.GetItem<Realm>(RealmKey);
  public static Session? GetSession(this HttpContext context) => context.GetItem<Session>(SessionKey);
  public static User? GetUser(this HttpContext context) => context.GetItem<User>(UserKey);
  private static T? GetItem<T>(this HttpContext context, object key) => context.Items.TryGetValue(key, out object? value) ? (T?)value : default;

  public static void SetApiKey(this HttpContext context, ApiKey? apiKey) => context.SetItem(ApiKeyKey, apiKey);
  public static void SetRealm(this HttpContext context, Realm? realm) => context.SetItem(RealmKey, realm);
  public static void SetSession(this HttpContext context, Session? session) => context.SetItem(SessionKey, session);
  public static void SetUser(this HttpContext context, User? user) => context.SetItem(UserKey, user);
  private static void SetItem(this HttpContext context, object key, object? value)
  {
    if (value == null)
    {
      context.Items.Remove(key);
    }
    else
    {
      context.Items[key] = value;
    }
  }

  public static string? GetSessionId(this HttpContext context)
  {
    byte[]? bytes = context.Session.Get(SessionIdKey);

    return bytes == null ? null : Encoding.UTF8.GetString(bytes);
  }
  public static bool IsSignedIn(this HttpContext context) => context.GetSessionId() != null;
  public static void SignIn(this HttpContext context, Session session)
  {
    context.Session.Set(SessionIdKey, Encoding.UTF8.GetBytes(session.Id));

    if (session.RefreshToken != null)
    {
      CookiesSettings cookiesSettings = context.RequestServices.GetRequiredService<CookiesSettings>();
      CookieOptions options = new()
      {
        HttpOnly = cookiesSettings.RefreshToken.HttpOnly,
        MaxAge = cookiesSettings.RefreshToken.MaxAge,
        SameSite = cookiesSettings.RefreshToken.SameSite,
        Secure = cookiesSettings.RefreshToken.Secure
      };
      context.Response.Cookies.Append(Cookies.RefreshToken, session.RefreshToken, options);
    }

    context.SetSession(session);
    context.SetUser(session.User);
  }
  public static void SignOut(this HttpContext context)
  {
    context.Session.Clear();

    context.Response.Cookies.Delete(Cookies.RefreshToken);
  }
}
