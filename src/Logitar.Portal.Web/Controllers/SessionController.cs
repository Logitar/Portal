using Logitar.Portal.Contracts.Sessions;
using Logitar.Portal.Web.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Logitar.Portal.Web.Controllers;

[ApiExplorerSettings(IgnoreApi = true)]
[Authorize(Policy = Policies.PortalActor)]
[Route("sessions")]
public class SessionController : Controller
{
  private readonly ISessionService _sessionService;

  public SessionController(ISessionService sessionService)
  {
    _sessionService = sessionService;
  }

  [HttpGet]
  public ActionResult SessionList()
  {
    return View();
  }

  [HttpGet("{id}")]
  public async Task<ActionResult> SessionView(Guid id, CancellationToken cancellationToken = default)
  {
    Session? session = await _sessionService.GetAsync(id, cancellationToken);
    if (session == null)
    {
      return NotFound();
    }

    return View(session);
  }
}
