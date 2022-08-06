using AutoMapper;
using Logitar.Portal.Core;
using Logitar.Portal.Core.Sessions;
using Logitar.Portal.Core.Sessions.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Logitar.Portal.Web.Controllers.Api
{
  [ApiController]
  [Authorize(Policy = Constants.Policies.PortalIdentity)]
  [Route("api/sessions")]
  public class SessionApiController : ControllerBase
  {
    private readonly IMapper _mapper;
    private readonly ISessionService _sessionService;

    public SessionApiController(IMapper mapper, ISessionService sessionService)
    {
      _mapper = mapper;
      _sessionService = sessionService;
    }

    [Authorize(Policy = Constants.Policies.AuthenticatedUser)]
    [HttpGet]
    public async Task<ActionResult<ListModel<SessionSummary>>> GetSessionsAsync(bool? isActive, bool? isPersistent, string? realm, Guid? userId,
      SessionSort? sort, bool desc,
      int? index, int? count,
      CancellationToken cancellationToken)
    {
      ListModel<SessionModel> sessions = await _sessionService.GetAsync(isActive, isPersistent, realm, userId,
        sort, desc,
        index, count,
        cancellationToken);

      return Ok(ListModel<SessionSummary>.From(sessions, _mapper));
    }

    [HttpPatch("{id}/sign/out")]
    public async Task<ActionResult<SessionModel>> SignOutAsync(Guid id, CancellationToken cancellationToken)
    {
      return Ok(await _sessionService.SignOutAsync(id, cancellationToken));
    }
  }
}
