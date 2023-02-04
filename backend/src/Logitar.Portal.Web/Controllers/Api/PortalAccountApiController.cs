using Logitar.Portal.Application.Accounts;
using Logitar.Portal.Contracts.Accounts;
using Logitar.Portal.Contracts.Sessions;
using Logitar.Portal.Web.Extensions;
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
      payload.IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
      payload.AdditionalInformation = JsonSerializer.Serialize(HttpContext.Request.Headers);

      SessionModel session = await _accountService.SignInAsync(payload, realm: null, cancellationToken);
      HttpContext.SignIn(session);

      return NoContent();
    }
  }
}
