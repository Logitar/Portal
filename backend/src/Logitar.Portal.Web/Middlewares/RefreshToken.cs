using Logitar.Portal.Core.Accounts;
using Logitar.Portal.Core.Accounts.Payloads;
using Logitar.Portal.Core.Sessions.Models;
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
      if (!context.GetSessionId().HasValue)
      {
        if (context.Request.Cookies.TryGetValue(Constants.Cookies.RenewToken, out string? renewToken) && renewToken != null)
        {
          try
          {
            string? ipAddress = context.Connection.RemoteIpAddress?.ToString();
            string? additionalInformation = JsonSerializer.Serialize(context.Request.Headers);

            var payload = new RenewSessionPayload
            {
              RenewToken = renewToken
            };
            SessionModel session = await accountService.RenewSessionAsync(payload, realm: null, ipAddress, additionalInformation);
            context.SetSession(session);
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
