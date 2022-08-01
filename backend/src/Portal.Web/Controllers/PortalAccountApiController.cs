using Microsoft.AspNetCore.Mvc;
using Portal.Core.Accounts;
using Portal.Core.Accounts.Payloads;
using Portal.Core.Sessions.Models;
using System.Text.Json;

namespace Portal.Web.Controllers
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
    public async Task<ActionResult> SignInAsync([FromBody] SignInPayload payload, CancellationToken cancellationToken)
    {
      string? ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
      string? additionalInformation = JsonSerializer.Serialize(HttpContext.Request.Headers);

      SessionModel session = await _accountService.SignInAsync(payload, realm: null, ipAddress, additionalInformation, cancellationToken);
      HttpContext.SetSession(session);

      return NoContent();
    }
  }
}
