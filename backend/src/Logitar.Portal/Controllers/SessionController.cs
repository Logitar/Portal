﻿using Logitar.Portal.Contracts.Sessions;
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

  [HttpPut("renew")]
  public async Task<ActionResult<Session>> RenewAsync([FromBody] RenewSessionPayload payload, CancellationToken cancellationToken)
  {
    return Ok(await _sessionService.RenewAsync(payload, cancellationToken));
  }

  [HttpPost("sign/in")]
  public async Task<ActionResult<Session>> SignInAsync([FromBody] SignInSessionPayload payload, CancellationToken cancellationToken)
  {
    Session session = await _sessionService.SignInAsync(payload, cancellationToken);
    Uri uri = new($"{Request.Scheme}://{Request.Host}/sessions/{session.Id}");

    return Created(uri, session);
  }
}