using Logitar.Portal.Contracts.Constants;
using Logitar.Portal.Contracts.Messages;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Logitar.Portal.Web.Controllers;

[ApiController]
[Authorize(Policy = Policies.PortalActor)]
[Route("api/messages")]
public class MessageController : ControllerBase
{
  private readonly IMessageService _messageService;

  public MessageController(IMessageService messageService)
  {
    _messageService = messageService;
  }

  [HttpPost("send")]
  public async Task<ActionResult<SentMessages>> SendAsync([FromBody] SendMessagePayload payload, CancellationToken cancellationToken)
  {
    SentMessages sentMessages = await _messageService.SendAsync(payload, cancellationToken);
    if (sentMessages.Ids.Count() == 1)
    {
      Uri uri = new($"{Request.Scheme}://{Request.Host}/api/messages/{sentMessages.Ids.Single()}");

      return Created(uri, sentMessages);
    }

    return Ok(sentMessages);
  }
}
