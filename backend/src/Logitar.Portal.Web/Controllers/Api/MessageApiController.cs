using AutoMapper;
using Logitar.Portal.Core;
using Logitar.Portal.Core.Emails.Messages;
using Logitar.Portal.Core.Emails.Messages.Models;
using Logitar.Portal.Core.Emails.Messages.Payloads;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Logitar.Portal.Web.Controllers.Api
{
  [ApiController]
  [Authorize(Policy = Constants.Policies.PortalIdentity)]
  [Route("api/messages")]
  public class MessageApiController : ControllerBase
  {
    private readonly IMapper _mapper;
    private readonly IMessageService _messageService;

    public MessageApiController(IMapper mapper, IMessageService messageService)
    {
      _mapper = mapper;
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
    public async Task<ActionResult<ListModel<MessageSummary>>> GetAsync(bool? hasErrors, bool? isDemo, string? realm, string? search, bool? succeeded, string? template,
      MessageSort? sort, bool desc,
      int? index, int? count,
      CancellationToken cancellationToken)
    {
      ListModel<MessageModel> messages = await _messageService.GetAsync(hasErrors, isDemo, realm, search, succeeded, template,
        sort, desc,
        index, count,
        cancellationToken);

      return Ok(ListModel<MessageSummary>.From(messages, _mapper));
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
