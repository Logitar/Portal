using Logitar.Portal.v2.Web.Extensions;

namespace Logitar.Portal.v2.Web.Middlewares;

public class RefreshSession
{
  private readonly RequestDelegate _next;

  public RefreshSession(RequestDelegate next)
  {
    _next = next;
  }

  public async Task InvokeAsync(HttpContext context)
  {
    if (!context.IsSignedIn())
    {
      if (context.Request.Cookies.TryGetValue(WebConstants.Cookies.RefreshToken, out string? refreshToken) && refreshToken != null)
      {
        try
        {
          // TODO(fpion): refresh session
          //RenewSessionPayload payload = new()
          //{
          //  AdditionalInformation = JsonSerializer.Serialize(context.Request.Headers),
          //  IpAddress = context.Connection.RemoteIpAddress?.ToString(),
          //  RenewToken = refreshToken
          //};
          //SessionModel session = await accountService.RenewSessionAsync(payload);
          //context.SetSession(session);
        }
        catch (Exception)
        {
          context.Response.Cookies.Delete(WebConstants.Cookies.RefreshToken);
        }
      }
    }

    await _next(context);
  }
}
