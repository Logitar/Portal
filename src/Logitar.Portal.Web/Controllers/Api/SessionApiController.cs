﻿using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Sessions;
using Logitar.Portal.Core.Realms;
using Logitar.Portal.Web.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Logitar.Portal.Web.Controllers.Api;

[ApiController]
[Authorize(Policy = Policies.PortalActor)]
[Route("api/sessions")]
public class SessionApiController : ControllerBase
{
  private readonly ISessionService _sessionService;

  public SessionApiController(ISessionService sessionService)
  {
    _sessionService = sessionService;
  }

  [HttpGet]
  public async Task<ActionResult<PagedList<Session>>> GetAsync(bool? isActive, bool? isPersistent, string? realm, Guid? userId,
      SessionSort? sort, bool isDescending, int? skip, int? limit, CancellationToken cancellationToken)
  {
    return Ok(await _sessionService.GetAsync(isActive, isPersistent, realm, userId,
      sort, isDescending, skip, limit, cancellationToken));
  }

  [HttpGet("{id}")]
  public async Task<ActionResult<Session>> GetAsync(Guid id, CancellationToken cancellationToken)
  {
    Session? session = await _sessionService.GetAsync(id, cancellationToken);
    if (session == null)
    {
      return NotFound(session);
    }

    return Ok(session);
  }

  [HttpPost("refresh")]
  public async Task<ActionResult<Session>> RefreshAsync(RefreshInput input, CancellationToken cancellationToken)
  {
    return Ok(await _sessionService.RefreshAsync(input, cancellationToken));
  }

  [HttpPost("sign/in")]
  public async Task<ActionResult<Session>> SignInAsync([FromBody] SignInInput input, CancellationToken cancellationToken)
  {
    if (input.Realm == RealmAggregate.PortalUniqueName)
    {
      return Forbid();
    }

    return Ok(await _sessionService.SignInAsync(input, cancellationToken));
  }

  [HttpPatch("{id}/sign/out")]
  public async Task<ActionResult<Session>> SignOutAsync(Guid id, CancellationToken cancellationToken)
  {
    return Ok(await _sessionService.SignOutAsync(id, cancellationToken));
  }

  [HttpPatch("sign/out/user/{id}")]
  public async Task<ActionResult<IEnumerable<Session>>> SignOutUserAsync(Guid id, CancellationToken cancellationToken)
  {
    return Ok(await _sessionService.SignOutUserAsync(id, cancellationToken));
  }
}
