using Logitar.Portal.Contracts;
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

  [HttpGet("{id}")]
  public async Task<ActionResult<Message>> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    Message? message = await _messageService.ReadAsync(id, cancellationToken);
    return message == null ? NotFound() : Ok(message);
  }

  [HttpPost("search")]
  public async Task<ActionResult<SearchResults<Message>>> SearchAsync([FromBody] SearchMessagesPayload payload, CancellationToken cancellationToken)
  {
    return Ok(await _messageService.SearchAsync(payload, cancellationToken));
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

  [HttpPost("send/demo")]
  public async Task<ActionResult<Message>> SendDemoAsync([FromBody] SendDemoMessagePayload payload, CancellationToken cancellationToken)
  {
    Message message = await _messageService.SendDemoAsync(payload, cancellationToken);
    Uri uri = new($"{Request.Scheme}://{Request.Host}/api/messages/{message.Id}");

    return Created(uri, message);
  }
}
