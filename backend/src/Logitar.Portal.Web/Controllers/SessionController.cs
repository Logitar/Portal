﻿using Logitar.Portal.Application.Sessions;
using Logitar.Portal.Contracts.Search;
using Logitar.Portal.Contracts.Sessions;
using Logitar.Portal.Web.Extensions;
using Logitar.Portal.Web.Models.Sessions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Logitar.Portal.Web.Controllers;

[ApiController]
[Authorize]
[Route("api/sessions")]
public class SessionController : ControllerBase
{
  private readonly ISessionService _sessionService;

  public SessionController(ISessionService sessionService)
  {
    _sessionService = sessionService;
  }

  [HttpPost]
  public async Task<ActionResult<SessionModel>> CreateAsync([FromBody] CreateSessionPayload payload, CancellationToken cancellationToken)
  {
    SessionModel session = await _sessionService.CreateAsync(payload, cancellationToken);
    return Created(BuildLocation(session), session);
  }

  [HttpGet("{id}")]
  public async Task<ActionResult<SessionModel>> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    SessionModel? session = await _sessionService.ReadAsync(id, cancellationToken);
    return session == null ? NotFound() : Ok(session);
  }

  [HttpPut("renew")]
  public async Task<ActionResult<SessionModel>> RenewAsync([FromBody] RenewSessionPayload payload, CancellationToken cancellationToken)
  {
    return Ok(await _sessionService.RenewAsync(payload, cancellationToken));
  }

  [HttpGet]
  public async Task<ActionResult<SearchResults<SessionModel>>> SearchAsync([FromQuery] SearchSessionsModel model, CancellationToken cancellationToken)
  {
    SearchSessionsPayload payload = model.ToPayload();
    return Ok(await _sessionService.SearchAsync(payload, cancellationToken));
  }

  [HttpPost("sign/in")]
  public async Task<ActionResult<SessionModel>> SignInAsync([FromBody] SignInSessionPayload payload, CancellationToken cancellationToken)
  {
    SessionModel session = await _sessionService.SignInAsync(payload, cancellationToken);
    return Created(BuildLocation(session), session);
  }

  [HttpPatch("{id}/sign/out")]
  public async Task<ActionResult<SessionModel>> SignOutAsync(Guid id, CancellationToken cancellationToken)
  {
    SessionModel? session = await _sessionService.SignOutAsync(id, cancellationToken);
    return session == null ? NotFound() : Ok(session);
  }

  private Uri BuildLocation(SessionModel session) => HttpContext.BuildLocation("sessions/{id}", new Dictionary<string, string> { ["id"] = session.Id.ToString() });
}
