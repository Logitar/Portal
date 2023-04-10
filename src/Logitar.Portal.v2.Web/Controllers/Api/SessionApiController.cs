using Logitar.Portal.v2.Contracts.Sessions;
using Microsoft.AspNetCore.Mvc;

namespace Logitar.Portal.v2.Web.Controllers.Api;

[ApiController]
//[Authorize(Policy = Constants.Policies.PortalIdentity)] // TODO(fpion): Authorization
[Route("api/sessions")]
public class SessionApiController : ControllerBase
{
  private readonly ISessionService _sessionService;

  public SessionApiController(ISessionService sessionService)
  {
    _sessionService = sessionService;
  }

  [HttpPost("refresh")]
  public async Task<ActionResult<Session>> RefreshAsync(RefreshInput input, CancellationToken cancellationToken)
  {
    return Ok(await _sessionService.RefreshAsync(input, cancellationToken));
  }

  [HttpPost("sign/in")]
  public async Task<ActionResult<Session>> SignInAsync([FromBody] SignInInput input, CancellationToken cancellationToken)
  {
    return Ok(await _sessionService.SignInAsync(input, cancellationToken));
  }

  [HttpPatch("{id}/sign/out")]
  public async Task<ActionResult<Session>> SignOutAsync(Guid id, CancellationToken cancellationToken)
  {
    return Ok(await _sessionService.SignOutAsync(id, cancellationToken));
  }
}
