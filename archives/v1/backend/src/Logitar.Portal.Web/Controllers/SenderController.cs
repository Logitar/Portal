using Logitar.Portal.Application.Emails.Senders;
using Logitar.Portal.Core.Emails.Senders.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Logitar.Portal.Web.Controllers
{
  [ApiExplorerSettings(IgnoreApi = true)]
  [Authorize(Policy = Constants.Policies.PortalIdentity)]
  [Route("senders")]
  public class SenderController : Controller
  {
    private readonly ISenderService _senderService;

    public SenderController(ISenderService senderService)
    {
      _senderService = senderService;
    }

    [HttpGet("/create-sender")]
    public ActionResult CreateSender()
    {
      return View(nameof(SenderEdit));
    }

    [HttpGet]
    public ActionResult SenderList()
    {
      return View();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult> SenderEdit(Guid id, CancellationToken cancellationToken = default)
    {
      SenderModel? sender = await _senderService.GetAsync(id, cancellationToken);
      if (sender == null)
      {
        return NotFound();
      }

      return View(sender);
    }
  }
}
