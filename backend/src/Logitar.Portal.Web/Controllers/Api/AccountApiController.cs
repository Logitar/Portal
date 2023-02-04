using Logitar.Portal.Application.Accounts;
using Logitar.Portal.Contracts.Accounts;
using Logitar.Portal.Contracts.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Logitar.Portal.Web.Controllers.Api
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

    [Authorize(Policy = Constants.Policies.Session)]
    [HttpPost("sign/out")]
    public async Task<ActionResult> SignOutAsync(CancellationToken cancellationToken)
    {
      await _accountService.SignOutAsync(cancellationToken);

      return NoContent();
    }
  }
}
