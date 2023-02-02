using Logitar.Portal.Application.Accounts;
using Logitar.Portal.Contracts.Accounts;
using Logitar.Portal.Contracts.Sessions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Logitar.Portal.Web.Controllers.Api
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
      SessionModel session = await _accountService.RenewSessionAsync(payload, id, cancellationToken);

      return Ok(session);
    }

    [HttpPost("sign/in")]
    public async Task<ActionResult<SessionModel>> SignInAsync(string id, [FromBody] SignInPayload payload, CancellationToken cancellationToken)
    {
      return Ok(await _accountService.SignInAsync(payload, id, cancellationToken));
    }
  }
}
