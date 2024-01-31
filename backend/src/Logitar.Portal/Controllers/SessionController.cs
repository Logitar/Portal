using Logitar.Portal.Contracts.Search;
using Logitar.Portal.Contracts.Sessions;
using Logitar.Portal.Models.Sessions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Logitar.Portal.Controllers;

[ApiController]
[Authorize]
[Route("sessions")]
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
    return Created(GetLocation(session), session);
  }

  [HttpGet("{id}")]
  public async Task<ActionResult<Session>> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    Session? session = await _sessionService.ReadAsync(id, cancellationToken);
    return session == null ? NotFound() : Ok(session);
  }

  [HttpPut("renew")]
  public async Task<ActionResult<Session>> RenewAsync([FromBody] RenewSessionPayload payload, CancellationToken cancellationToken)
  {
    return Ok(await _sessionService.RenewAsync(payload, cancellationToken));
  }

  [HttpGet]
  public async Task<ActionResult<SearchResults<Session>>> SearchAsync([FromQuery] SearchSessionsModel model, CancellationToken cancellationToken)
  {
    SearchSessionsPayload payload = model.ToPayload();
    return Ok(await _sessionService.SearchAsync(payload, cancellationToken));
  }

  [HttpPost("sign/in")]
  public async Task<ActionResult<Session>> SignInAsync([FromBody] SignInSessionPayload payload, CancellationToken cancellationToken)
  {
    Session session = await _sessionService.SignInAsync(payload, cancellationToken);
    return Created(GetLocation(session), session);
  }

  [HttpPatch("{id}/sign/out")]
  public async Task<ActionResult<Session>> SignOutAsync(Guid id, CancellationToken cancellationToken)
  {
    Session? session = await _sessionService.SignOutAsync(id, cancellationToken);
    return session == null ? NotFound() : Ok(session);
  }

  private Uri GetLocation(Session session) => new($"{Request.Scheme}://{Request.Host}/sessions/{session.Id}");
}
