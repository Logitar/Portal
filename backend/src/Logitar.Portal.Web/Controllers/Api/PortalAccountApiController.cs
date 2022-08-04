using Logitar.Portal.Core.Accounts;
using Logitar.Portal.Core.Accounts.Payloads;
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
    public async Task<ActionResult> SignInAsync([FromBody] SignInPayload payload, CancellationToken cancellationToken)
    {
      string? ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
      string additionalInformation = JsonSerializer.Serialize(HttpContext.Request.Headers);

      var session = await _accountService.SignInAsync(payload, realm: null, ipAddress, additionalInformation, cancellationToken);
      HttpContext.SetSession(session);

      return NoContent();
    }
  }
}
