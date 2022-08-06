using AutoMapper;
using Logitar.Portal.Core;
using Logitar.Portal.Core.Accounts;
using Logitar.Portal.Core.Accounts.Payloads;
using Logitar.Portal.Core.Sessions;
using Logitar.Portal.Core.Sessions.Models;
using Logitar.Portal.Core.Users.Models;
using Logitar.Portal.Core.Users.Payloads;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Logitar.Portal.Web.Controllers.Api
{
  [ApiController]
  [Route("api/account")]
  public class AccountApiController : ControllerBase
  {
    private readonly IAccountService _accountService;
    private readonly IMapper _mapper;

    public AccountApiController(IAccountService accountService, IMapper mapper)
    {
      _accountService = accountService;
      _mapper = mapper;
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

    [Authorize(Policy = Constants.Policies.AuthenticatedUser)]
    [HttpGet("sessions")]
    public async Task<ActionResult<ListModel<SessionSummary>>> GetSessionsAsync(bool? isActive, bool? isPersistent,
      SessionSort? sort, bool desc,
      int? index, int? count,
      CancellationToken cancellationToken)
    {
      ListModel<SessionModel> sessions = await _accountService.GetSessionsAsync(isActive, isPersistent,
        sort, desc,
        index, count,
        cancellationToken);

      return Ok(ListModel<SessionSummary>.From(sessions, _mapper));
    }

    [Authorize(Policy = Constants.Policies.Session)]
    [HttpPost("sign/out")]
    public async Task<ActionResult> SignOutAsync(CancellationToken cancellationToken)
    {
      await _accountService.SignOutAsync(cancellationToken);

      return NoContent();
    }

    [Authorize(Policy = Constants.Policies.AuthenticatedUser)]
    [HttpPost("sign/out/all")]
    public async Task<ActionResult> SignOutAllAsync(CancellationToken cancellationToken)
    {
      await _accountService.SignOutAllAsync(cancellationToken);

      HttpContext.Session.Clear();
      HttpContext.Response.Cookies.Delete(Constants.Cookies.RenewToken);

      return NoContent();
    }

    [Authorize(Policy = Constants.Policies.AuthenticatedUser)]
    [HttpPost("sign/out/{id}")]
    public async Task<ActionResult> SignOutAsync(Guid id, CancellationToken cancellationToken)
    {
      await _accountService.SignOutAsync(id, cancellationToken);

      if (id == HttpContext.GetSessionId())
      {
        HttpContext.Session.Clear();
        HttpContext.Response.Cookies.Delete(Constants.Cookies.RenewToken);
      }

      return NoContent();
    }
  }
}
