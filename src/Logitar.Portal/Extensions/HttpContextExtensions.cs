using Logitar.Portal.Constants;
using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Sessions;
using Microsoft.Extensions.Primitives;

namespace Logitar.Portal.Extensions;

internal static class HttpContextExtensions
{
  private const string SessionIdKey = "SessionId";

  public static IEnumerable<CustomAttribute> GetSessionCustomAttributes(this HttpContext context)
  {
    List<CustomAttribute> customAttributes = new(capacity: 2)
    {
      new("AdditionalInformation", JsonSerializer.Serialize(context.Request.Headers))
    };

    string? ipAddress = null;
    if (context.Request.Headers.TryGetValue("X-Forwarded-For", out StringValues xForwardedFor))
    {
      ipAddress = xForwardedFor.Single()?.Split(':').First();
    }
    ipAddress ??= context.Connection.RemoteIpAddress?.ToString();
    if (ipAddress != null)
    {
      customAttributes.Add(new("IpAddress", ipAddress));
    }

    return customAttributes;
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
      context.Response.Cookies.Append(Cookies.RefreshToken, session.RefreshToken, Cookies.RefreshTokenOptions);
    }
  }
  public static void SignOut(this HttpContext context)
  {
    context.Session.Clear();

    context.Response.Cookies.Delete(Cookies.RefreshToken);
  }
}
