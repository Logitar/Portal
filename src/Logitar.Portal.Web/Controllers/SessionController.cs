using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Constants;
using Logitar.Portal.Contracts.Sessions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Logitar.Portal.Web.Controllers;

[ApiController]
[Authorize(Policy = Policies.PortalActor)]
[Route("api/sessions")]
public class SessionController : ControllerBase
{
  private readonly ISessionService _sessionService;

  public SessionController(ISessionService sessionService)
  {
    _sessionService = sessionService;
  }

  [HttpPost]
  public async Task<ActionResult<Session>> CreateAsync([FromBody] CreateSessionPayload payload, CancellationToken cancellationToken)
  {
    Session session = await _sessionService.CreateAsync(payload, cancellationToken);
    Uri uri = new($"{Request.Scheme}://{Request.Host}/api/sessions/{session.Id}");

    return Created(uri, session);
  }

  [HttpGet("{id}")]
  public async Task<ActionResult<Session>> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    Session? session = await _sessionService.ReadAsync(id, cancellationToken);
    return session == null ? NotFound() : Ok(session);
  }

  [HttpPost("renew")]
  public async Task<ActionResult<Session>> RenewAsync([FromBody] RenewPayload payload, CancellationToken cancellationToken)
  {
    return Ok(await _sessionService.RenewAsync(payload, cancellationToken));
  }

  [HttpPost("search")]
  public async Task<ActionResult<SearchResults<Session>>> SearchAsync([FromBody] SearchSessionsPayload payload, CancellationToken cancellationToken)
  {
    return Ok(await _sessionService.SearchAsync(payload, cancellationToken));
  }

  [HttpPost("sign/in")]
  public async Task<ActionResult<Session>> SignInAsync([FromBody] SignInPayload payload, CancellationToken cancellationToken)
  {
    Session session = await _sessionService.SignInAsync(payload, cancellationToken);
    Uri uri = new($"{Request.Scheme}://{Request.Host}/api/sessions/{session.Id}");

    return Created(uri, session);
  }

  [HttpPatch("{id}/sign/out")]
  public async Task<ActionResult<Session>> SignOutAsync(Guid id, CancellationToken cancellationToken)
  {
    Session? session = await _sessionService.SignOutAsync(id, cancellationToken);
    return session == null ? NotFound() : Ok(session);
  }
}
