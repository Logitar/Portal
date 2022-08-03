using Logitar.Portal.Core.Accounts;
using Logitar.Portal.Core.Accounts.Payloads;
using Logitar.Portal.Core.Sessions.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace Logitar.Portal.Web.Controllers
{
  [ApiController]
  [Authorize(Policy = Constants.Policies.ApiKey)]
  [Route("api/realms/{id}/account")]
  public class RealmAccountApiController : ControllerBase
  {
    private readonly IAccountService _accountService;

    public RealmAccountApiController(IAccountService accountService)
    {
      _accountService = accountService;
    }

    [HttpPost("password/recover")]
    public async Task<ActionResult> RecoverPasswordAsync(string id, [FromBody] RecoverPasswordPayload payload, CancellationToken cancellationToken)
    {
      await _accountService.RecoverPasswordAsync(payload, id, cancellationToken);

      return NoContent();
    }

    [HttpPost("password/reset")]
    public async Task<ActionResult> ResetPasswordAsync(string id, [FromBody] ResetPasswordPayload payload, CancellationToken cancellationToken)
    {
      await _accountService.ResetPasswordAsync(payload, id, cancellationToken);

      return NoContent();
    }

    [HttpPost("renew")]
    public async Task<ActionResult<SessionModel>> RenewSessionAsync(string id, RenewSessionPayload payload, CancellationToken cancellationToken)
    {
      string? ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
      string? additionalInformation = JsonSerializer.Serialize(HttpContext.Request.Headers);

      SessionModel session = await _accountService.RenewSessionAsync(payload, id, ipAddress, additionalInformation, cancellationToken);
      if (session.User?.Realm == null)
      {
        HttpContext.SetSession(session);

        return NoContent();
      }

      return Ok(session);
    }

    [HttpPost("sign/in")]
    public async Task<ActionResult<SessionModel>> SignInAsync(string id, [FromBody] SignInPayload payload, CancellationToken cancellationToken)
    {
      string? ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
      string? additionalInformation = JsonSerializer.Serialize(HttpContext.Request.Headers);

      return Ok(await _accountService.SignInAsync(payload, id, ipAddress, additionalInformation, cancellationToken));
    }
  }
}
