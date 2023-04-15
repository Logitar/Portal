﻿using Logitar.Portal.Contracts.Sessions;
using Logitar.Portal.Web.Constants;
using Logitar.Portal.Web.Extensions;

namespace Logitar.Portal.Web.Middlewares;

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

      if (request.Cookies.TryGetValue(Cookies.RefreshToken, out string? refreshToken))
      {
        try
        {
          RefreshInput input = new()
          {
            RefreshToken = refreshToken,
            IpAddress = context.GetClientIpAddress(),
            AdditionalInformation = context.GetAdditionalInformation()
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
