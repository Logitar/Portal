using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Portal.Core;
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

    [HttpPost("demo")]
    public async Task<ActionResult<MessageModel>> SendDemoAsync(SendDemoMessagePayload payload, CancellationToken cancellationToken)
    {
      return Ok(await _messageService.SendDemoAsync(payload, cancellationToken));
    }

    [HttpGet]
    public async Task<ActionResult<ListModel<MessageSummary>>> GetAsync(bool? hasErrors, Guid? realmId, string? search, bool? succeeded, Guid? templateId,
      MessageSort? sort, bool desc,
      int? index, int? count,
      CancellationToken cancellationToken)
    {
      return Ok(await _messageService.GetAsync(hasErrors, realmId, search, succeeded, templateId,
        sort, desc,
        index, count,
        cancellationToken));
    }
    
    [HttpGet("{id}")]
    public async Task<ActionResult<MessageModel>> GetAsync(Guid id, CancellationToken cancellationToken)
    {
      MessageModel? message = await _messageService.GetAsync(id, cancellationToken);
      if (message == null)
      {
        return NotFound();
      }

      return Ok(message);
    }
  }
}
