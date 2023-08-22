using Logitar.Portal.Constants;
using Logitar.Portal.Contracts.Sessions;
using Logitar.Portal.Extensions;

namespace Logitar.Portal.Middlewares;

internal class RefreshSession
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

      if (request.Cookies.TryGetValue(Cookies.RefreshToken, out string? refreshToken))
      {
        try
        {
          RenewSessionPayload payload = new()
          {
            RefreshToken = refreshToken,
            CustomAttributes = context.GetSessionCustomAttributes()
          };
          Session session = await sessionService.RenewAsync(payload);
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
