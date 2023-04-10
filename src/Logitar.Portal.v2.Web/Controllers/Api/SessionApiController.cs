using Logitar.Portal.v2.Contracts.Sessions;
using Microsoft.AspNetCore.Mvc;

namespace Logitar.Portal.v2.Web.Controllers.Api;

[ApiController]
[Route("api/sessions")]
public class SessionApiController : ControllerBase
{
  private readonly ISessionService _sessionService;

  public SessionApiController(ISessionService sessionService)
  {
    _sessionService = sessionService;
  }

  //[Authorize(Policy = Constants.Policies.PortalIdentity)] // TODO(fpion): Authorization
  [HttpPost("refresh")]
  public async Task<ActionResult<Session>> RefreshAsync(RefreshInput input, CancellationToken cancellationToken)
  {
    return Ok(await _sessionService.RefreshAsync(input, cancellationToken));
  }

  //[Authorize(Policy = Constants.Policies.PortalIdentity)] // TODO(fpion): Authorization
  [HttpPost("sign/in")]
  public async Task<ActionResult<Session>> SignInAsync([FromBody] SignInInput input, CancellationToken cancellationToken)
  {
    return Ok(await _sessionService.SignInAsync(input, cancellationToken));
  }
}
