using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.ApiKeys;
using Logitar.Portal.Contracts.Constants;
using Logitar.Portal.Contracts.Sessions;
using Logitar.Portal.Contracts.Users;
using Logitar.Portal.Web.Settings;
using Microsoft.Extensions.Primitives;

namespace Logitar.Portal.Web.Extensions;

internal static class HttpContextExtensions
{
  private const string ApiKeyKey = "ApiKey";
  private const string SessionIdKey = "SessionId";
  private const string SessionKey = "Session";
  private const string UserKey = "User";

  public static string GetAdditionalInformation(this HttpContext context)
  {
    return JsonSerializer.Serialize(context.Request.Headers);
  }
  public static string? GetClientIpAddress(this HttpContext context)
  {
    string? ipAddress = null;

    if (context.Request.Headers.TryGetValue("X-Forwarded-For", out StringValues xForwardedFor))
    {
      ipAddress = xForwardedFor.Single()?.Split(':').First();
    }
    ipAddress ??= context.Connection.RemoteIpAddress?.ToString();

    return ipAddress;
  }
  public static IEnumerable<CustomAttribute> GetSessionCustomAttributes(this HttpContext context)
  {
    List<CustomAttribute> customAttributes = new(capacity: 2)
    {
      new CustomAttribute("AdditionalInformation", context.GetAdditionalInformation())
    };

    string? ipAddress = context.GetClientIpAddress();
    if (ipAddress != null)
    {
      customAttributes.Add(new CustomAttribute("IpAddress", ipAddress));
    }

    return customAttributes;
  }

  public static ApiKey? GetApiKey(this HttpContext context) => context.GetItem<ApiKey>(ApiKeyKey);
  public static Session? GetSession(this HttpContext context) => context.GetItem<Session>(SessionKey);
  public static User? GetUser(this HttpContext context) => context.GetItem<User>(UserKey);
  private static T? GetItem<T>(this HttpContext context, object key)
  {
    return context.Items.TryGetValue(key, out object? value) ? (T?)value : default;
  }

  public static void SetApiKey(this HttpContext context, ApiKey? apiKey) => context.SetItem(ApiKeyKey, apiKey);
  public static void SetSession(this HttpContext context, Session? session) => context.SetItem(SessionKey, session);
  public static void SetUser(this HttpContext context, User? user) => context.SetItem(UserKey, user);
  private static void SetItem<T>(this HttpContext context, object key, T? value)
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

    context.SetUser(session.User);
  }
  public static void SignOut(this HttpContext context)
  {
    context.Session.Clear();

    context.Response.Cookies.Delete(Cookies.RefreshToken);
  }
}
