﻿using Logitar.Portal.Application.Messages;
using Logitar.Portal.Contracts.Messages;
using Logitar.Portal.Contracts.Search;
using Logitar.Portal.Web.Models.Messages;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Logitar.Portal.Web.Controllers;

[ApiController]
[Authorize]
[Route("api/messages")]
public class MessageController : ControllerBase
{
  private readonly IMessageService _messageService;

  public MessageController(IMessageService messageService)
  {
    _messageService = messageService;
  }

  [HttpGet("{id}")]
  public async Task<ActionResult<MessageModel>> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    MessageModel? message = await _messageService.ReadAsync(id, cancellationToken);
    return message == null ? NotFound() : Ok(message);
  }

  [HttpGet]
  public async Task<ActionResult<SearchResults<MessageModel>>> SearchAsync([FromQuery] SearchMessagesModel model, CancellationToken cancellationToken)
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
