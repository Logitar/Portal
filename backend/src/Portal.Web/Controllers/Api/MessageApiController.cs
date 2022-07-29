using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Portal.Core.Emails.Messages;
using Portal.Core.Emails.Messages.Models;
using Portal.Core.Emails.Messages.Payloads;

namespace Portal.Web.Controllers.Api
{
  [ApiController]
  [Authorize(Policy = Constants.Policies.PortalIdentity)]
  [Route("api/messages")]
  public class MessageApiController : ControllerBase
  {
    private readonly IMessageService _messageService;

    public MessageApiController(IMessageService messageService)
    {
      _messageService = messageService;
    }

    [HttpPost]
    public async Task<ActionResult<SentMessagesModel>> SendAsync(SendMessagePayload payload, CancellationToken cancellationToken)
    {
      return Ok(await _messageService.SendAsync(payload, cancellationToken));
    }
  }
}
