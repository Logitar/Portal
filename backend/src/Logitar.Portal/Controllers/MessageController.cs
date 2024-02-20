using Logitar.Portal.Contracts.Messages;
using Logitar.Portal.Contracts.Search;
using Logitar.Portal.Models.Messages;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Logitar.Portal.Controllers;

[ApiController]
[Authorize]
[Route("messages")]
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

  [HttpGet]
  public async Task<ActionResult<SearchResults<Message>>> SearchAsync([FromQuery] SearchMessagesModel model, CancellationToken cancellationToken)
  {
    SearchMessagesPayload payload = model.ToPayload();
    return Ok(await _messageService.SearchAsync(payload, cancellationToken));
  }

  [HttpPost]
  public async Task<ActionResult<SentMessages>> SendAsync([FromBody] SendMessagePayload payload, CancellationToken cancellationToken)
  {
    return Ok(await _messageService.SendAsync(payload, cancellationToken));
  }
}
