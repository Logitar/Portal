using Microsoft.AspNetCore.Mvc;

namespace Logitar.Portal.v2.Web.Controllers.Api;

/// <summary>
/// TODO(fpion): Accounts
/// </summary>
[ApiController]
//[Authorize(Policy = Constants.Policies.ApiKey)] // TODO(fpion): Authorization
[Route("api/realms/{id}/account")]
public class RealmAccountApiController : ControllerBase
{
  //private readonly IAccountService _accountService;

  //public RealmAccountApiController(IAccountService accountService)
  //{
  //  _accountService = accountService;
  //}

  //[HttpPost("password/recover")]
  //public async Task<ActionResult> RecoverPasswordAsync(string id, [FromBody] RecoverPasswordPayload payload, CancellationToken cancellationToken)
  //{
  //  await _accountService.RecoverPasswordAsync(payload, id, cancellationToken);

  //  return NoContent();
  //}

  //[HttpPost("password/reset")]
  //public async Task<ActionResult> ResetPasswordAsync(string id, [FromBody] ResetPasswordPayload payload, CancellationToken cancellationToken)
  //{
  //  await _accountService.ResetPasswordAsync(payload, id, cancellationToken);

  //  return NoContent();
  //}
}
