using Logitar.Portal.Application.Accounts;
using Logitar.Portal.Core.Accounts.Payloads;
using Logitar.Portal.Core.Sessions.Models;
using Logitar.Portal.Web.Models.Api.PortalAccount;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace Logitar.Portal.Web.Controllers.Api
{
  [ApiController]
  [Route("api/portal/account")]
  public class PortalAccountApiController : ControllerBase
  {
    private readonly IAccountService _accountService;

    public PortalAccountApiController(IAccountService accountService)
    {
      _accountService = accountService;
    }

    [HttpPost("sign/in")]
    public async Task<ActionResult> SignInAsync([FromBody] PortalSignInPayload payload, CancellationToken cancellationToken)
    {
      SessionModel session = await _accountService.SignInAsync(new SignInPayload
      {
        Username = payload.Username,
        Password = payload.Password,
        Remember = payload.Remember,
        IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString(),
        AdditionalInformation = JsonSerializer.Serialize(HttpContext.Request.Headers)
      }, realm: null, cancellationToken);
      HttpContext.SetSession(session);

      return NoContent();
    }
  }
}
