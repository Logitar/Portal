using Logitar.Portal.Constants;
using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Sessions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Logitar.Portal.Controllers.Api;

[ApiController]
[Authorize(Policies.PortalActor)]
[Route("api/sessions")]
public class SessionApiController : ControllerBase
{
  private readonly ISessionService _sessionService;

  public SessionApiController(ISessionService sessionService)
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
  public async Task<ActionResult<Session>> ReadAsync(string id, CancellationToken cancellationToken)
  {
    Session? session = await _sessionService.ReadAsync(id, cancellationToken);
    return session == null ? NotFound() : Ok(session);
  }

  [HttpPatch("renew")]
  public async Task<ActionResult<Session>> RenewAsync([FromBody] RenewSessionPayload payload, CancellationToken cancellationToken)
  {
    return Ok(await _sessionService.RenewAsync(payload, cancellationToken));
  }

  [HttpPost("search")]
  public async Task<ActionResult<SearchResults<Session>>> SearchAsync([FromBody] SearchSessionsPayload payload, CancellationToken cancellationToken)
  {
    return Ok(await _sessionService.SearchAsync(payload, cancellationToken));
  }

  [HttpPost("sign-in")]
  public async Task<ActionResult<Session>> SignInAsync([FromBody] SignInPayload payload, CancellationToken cancellationToken)
  {
    Session session = await _sessionService.SignInAsync(payload, cancellationToken);
    Uri uri = new($"{Request.Scheme}://{Request.Host}/api/sessions/{session.Id}");

    return Created(uri, session);
  }

  [HttpPatch("{id}")]
  public async Task<ActionResult<Session>> SignOutAsync(string id, CancellationToken cancellationToken)
  {
    Session? session = await _sessionService.SignOutAsync(id, cancellationToken);
    return session == null ? NotFound() : Ok(session);
  }
}
