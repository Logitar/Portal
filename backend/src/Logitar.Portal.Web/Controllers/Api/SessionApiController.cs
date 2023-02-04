using Logitar.Portal.Application.Sessions;
using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Sessions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Logitar.Portal.Web.Controllers.Api
{
  [ApiController]
  [Authorize(Policy = Constants.Policies.PortalIdentity)]
  [Route("api/sessions")]
  public class SessionApiController : ControllerBase
  {
    private readonly ISessionService _sessionService;

    public SessionApiController(ISessionService sessionService)
    {
      _sessionService = sessionService;
    }

    [HttpGet]
    public async Task<ActionResult<ListModel<SessionModel>>> GetAsync(bool? isActive, bool? isPersistent, string? realm, string? userId,
      SessionSort? sort, bool desc,
      int? index, int? count,
      CancellationToken cancellationToken)
    {
      return Ok(await _sessionService.GetAsync(isActive, isPersistent, realm, userId,
        sort, desc,
        index, count,
        cancellationToken));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<SessionModel>> GetAsync(string id, CancellationToken cancellationToken)
    {
      SessionModel? session = await _sessionService.GetAsync(id, cancellationToken);
      if (session == null)
      {
        return NotFound(session);
      }

      return Ok(session);
    }

    [HttpPatch("/api/users/{id}/sessions/sign/out")]
    public async Task<ActionResult<IEnumerable<SessionModel>>> SignOutAllAsync(string id, CancellationToken cancellationToken)
    {
      return Ok(await _sessionService.SignOutAllAsync(id, cancellationToken));
    }

    [HttpPatch("{id}/sign/out")]
    public async Task<ActionResult<SessionModel>> SignOutAsync(string id, CancellationToken cancellationToken)
    {
      return Ok(await _sessionService.SignOutAsync(id, cancellationToken));
    }
  }
}
