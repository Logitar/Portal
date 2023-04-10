using Logitar.Portal.v2.Contracts.Sessions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Logitar.Portal.v2.Web.Controllers;

[ApiExplorerSettings(IgnoreApi = true)]
[Authorize(Policy = Constants.Policies.PortalActor)]
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
