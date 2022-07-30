using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Portal.Core.Accounts;
using Portal.Core.Accounts.Payloads;
using Portal.Core.Sessions.Models;
using Portal.Core.Users.Models;
using Portal.Core.Users.Payloads;
using System.Text.Json;

namespace Portal.Web.Controllers.Api
{
  [ApiController]
  [Route("api/account")]
  public class AccountApiController : ControllerBase
  {
    private readonly IAccountService _accountService;

    public AccountApiController(IAccountService accountService)
    {
      _accountService = accountService;
    }

    [Authorize(Policy = Constants.Policies.AuthenticatedUser)]
    [HttpPost("password/change")]
    public async Task<ActionResult<UserModel>> ChangePasswordAsync([FromBody] ChangePasswordPayload payload, CancellationToken cancellationToken)
    {
      return Ok(await _accountService.ChangePasswordAsync(payload, cancellationToken));
    }

    [HttpPost("password/recover")]
    public async Task<ActionResult> RecoverPasswordAsync([FromBody] RecoverPasswordPayload payload, CancellationToken cancellationToken)
    {
      await _accountService.RecoverPasswordAsync(payload, cancellationToken);

      return NoContent();
    }

    [HttpPost("password/reset")]
    public async Task<ActionResult> ResetPasswordAsync([FromBody] ResetPasswordPayload payload, CancellationToken cancellationToken)
    {
      await _accountService.ResetPasswordAsync(payload, cancellationToken);

      return NoContent();
    }

    [Authorize(Policy = Constants.Policies.AuthenticatedUser)]
    [HttpGet("profile")]
    public async Task<ActionResult<UserModel>> GetProfileAsync(CancellationToken cancellationToken)
    {
      return Ok(await _accountService.GetProfileAsync(cancellationToken));
    }

    [Authorize(Policy = Constants.Policies.AuthenticatedUser)]
    [HttpPut("profile")]
    public async Task<ActionResult<UserModel>> SaveProfileAsync(UpdateUserPayload payload, CancellationToken cancellationToken)
    {
      return Ok(await _accountService.SaveProfileAsync(payload, cancellationToken));
    }

    [HttpPost("renew")]
    public async Task<ActionResult<SessionModel>> RenewSessionAsync(RenewSessionPayload payload, CancellationToken cancellationToken)
    {
      string? ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
      string? additionalInformation = JsonSerializer.Serialize(HttpContext.Request.Headers);

      SessionModel session = await _accountService.RenewSessionAsync(payload, ipAddress, additionalInformation, cancellationToken);
      if (session.User?.Realm == null)
      {
        HttpContext.SetSession(session);

        return NoContent();
      }

      return Ok(session);
    }

    [HttpPost("sign/in")]
    public async Task<ActionResult<SessionModel>> SignInAsync([FromBody] SignInPayload payload, CancellationToken cancellationToken)
    {
      string? ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
      string? additionalInformation = JsonSerializer.Serialize(HttpContext.Request.Headers);

      SessionModel session = await _accountService.SignInAsync(payload, ipAddress, additionalInformation, cancellationToken);
      if (session.User?.Realm == null)
      {
        HttpContext.SetSession(session);

        return NoContent();
      }

      return Ok(session);
    }

    [Authorize(Policy = Constants.Policies.AuthenticatedUser)]
    [HttpPost("sign/out")]
    public async Task<ActionResult> SignOutAsync(CancellationToken cancellationToken)
    {
      await _accountService.SignOutAsync(cancellationToken);

      return NoContent();
    }
  }
}
