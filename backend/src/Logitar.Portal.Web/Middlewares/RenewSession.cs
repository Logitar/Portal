using Logitar.Portal.Application.Accounts;
using Logitar.Portal.Contracts.Accounts;
using Logitar.Portal.Contracts.Sessions;
using Logitar.Portal.Web.Extensions;
using System.Text.Json;

namespace Logitar.Portal.Web.Middlewares
{
  internal class RenewSession
  {
    private readonly RequestDelegate _next;

    public RenewSession(RequestDelegate next)
    {
      _next = next;
    }

    public async Task InvokeAsync(HttpContext context, IAccountService accountService)
    {
      if (context.GetSessionId() == null)
      {
        if (context.Request.Cookies.TryGetValue(Constants.Cookies.RenewToken, out string? renewToken) && renewToken != null)
        {
          try
          {
            RenewSessionPayload payload = new()
            {
              AdditionalInformation = JsonSerializer.Serialize(context.Request.Headers),
              IpAddress = context.Connection.RemoteIpAddress?.ToString(),
              RenewToken = renewToken
            };
            SessionModel session = await accountService.RenewSessionAsync(payload);
            context.SignIn(session);
          }
          catch (Exception)
          {
            context.Response.Cookies.Delete(Constants.Cookies.RenewToken);
          }
        }
      }

      await _next(context);
    }
  }
}
