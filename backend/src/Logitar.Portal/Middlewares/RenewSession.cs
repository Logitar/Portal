﻿using Logitar.Portal.Contracts.Sessions;
using Logitar.Portal.Web.Constants;
using Logitar.Portal.Web.Extensions;

namespace Logitar.Portal.Middlewares;

internal class RenewSession
{
  private readonly RequestDelegate _next;

  public RenewSession(RequestDelegate next)
  {
    _next = next;
  }

  public async Task InvokeAsync(HttpContext context, ISessionService sessionService)
  {
    if (!context.GetSessionId().HasValue)
    {
      if (context.Request.Cookies.TryGetValue(Cookies.RefreshToken, out string? refreshToken) && refreshToken != null)
      {
        try
        {
          RenewSessionPayload payload = new(refreshToken, context.GetSessionCustomAttributes());
          Session session = await sessionService.RenewAsync(payload);
          context.SignIn(session);
        }
        catch (Exception)
        {
          context.Response.Cookies.Delete(Cookies.RefreshToken);
        }
      }
    }

    await _next(context);
  }
}
