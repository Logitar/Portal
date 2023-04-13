using Logitar.Portal.Contracts.Messages;
using Logitar.Portal.Web.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Logitar.Portal.Web.Controllers;

[ApiExplorerSettings(IgnoreApi = true)]
[Authorize(Policy = Policies.PortalActor)]
[Route("messages")]
public class MessageController : Controller
{
  private readonly IMessageService _messageService;

  public MessageController(IMessageService messageService)
  {
    _messageService = messageService;
  }

  [HttpGet]
  public ActionResult MessageList()
  {
    return View();
  }

  [HttpGet("{id}")]
  public async Task<ActionResult> MessageView(Guid id, CancellationToken cancellationToken)
  {
    Message? message = await _messageService.GetAsync(id, cancellationToken: cancellationToken);
    if (message == null)
    {
      return NotFound();
    }

    return View(message);
  }
}
