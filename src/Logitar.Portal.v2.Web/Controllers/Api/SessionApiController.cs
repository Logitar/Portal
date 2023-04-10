using Logitar.Portal.v2.Contracts.Sessions;
using Microsoft.AspNetCore.Mvc;

namespace Logitar.Portal.v2.Web.Controllers.Api;

/// <summary>
/// TODO(fpion): Renew
/// </summary>
[ApiController]
[Route("api/sessions")]
public class SessionApiController : ControllerBase
{
  private readonly ISessionService _sessionService;

  public SessionApiController(ISessionService sessionService)
  {
    _sessionService = sessionService;
  }

  //[Authorize(Policy = Constants.Policies.ApiKey)] // TODO(fpion): Authorization
  //[HttpPost("renew")]
  //public async Task<ActionResult<SessionModel>> RenewSessionAsync(string id, RenewSessionPayload payload, CancellationToken cancellationToken)
  //{
  //  SessionModel session = await _accountService.RenewSessionAsync(payload, id, cancellationToken);

  //  return Ok(session);
  //}

  //[Authorize(Policy = Constants.Policies.ApiKey)] // TODO(fpion): Authorization
  [HttpPost("sign/in")]
  public async Task<ActionResult<Session>> SignInAsync([FromBody] SignInInput input, CancellationToken cancellationToken)
  {
    return Ok(await _sessionService.SignInAsync(input, cancellationToken));
  }
}
