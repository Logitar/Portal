using Logitar.Portal.v2.Contracts;
using Logitar.Portal.v2.Contracts.Sessions;
using Logitar.Portal.v2.Web.Extensions;
using System.Text.Json;

namespace Logitar.Portal.v2.Web.Middlewares;

public class RefreshSession
{
  private readonly RequestDelegate _next;

  public RefreshSession(RequestDelegate next)
  {
    _next = next;
  }

  public async Task InvokeAsync(HttpContext context, ISessionService sessionService)
  {
    if (!context.IsSignedIn())
    {
      HttpRequest request = context.Request;

      if (request.Cookies.TryGetValue(Constants.Cookies.RefreshToken, out string? refreshToken))
      {
        try
        {
          List<CustomAttribute> customAttributes = new(capacity: 2)
          {
            new CustomAttribute
            {
              Key = "RequestHeaders",
              Value = JsonSerializer.Serialize(request.Headers)
            }
          };
          if (context.Connection.RemoteIpAddress != null)
          {
            customAttributes.Add(new CustomAttribute
            {
              Key = "RemoteIpAddress",
              Value = context.Connection.RemoteIpAddress.ToString()
            });
          }
          RefreshInput input = new()
          {
            RefreshToken = refreshToken,
            CustomAttributes = customAttributes
          };
          Session session = await sessionService.RefreshAsync(input);
          context.SignIn(session);
        }
        catch (Exception)
        {
          context.SignOut();
        }
      }
    }

    await _next(context);
  }
}
