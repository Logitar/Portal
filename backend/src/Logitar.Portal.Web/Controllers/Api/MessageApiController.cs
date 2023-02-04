using Logitar.Portal.Application.Messages;
using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Messages;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Logitar.Portal.Web.Controllers.Api
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

    [Authorize(Policy = Constants.Policies.PortalUser)]
    [HttpPost("demo")]
    public async Task<ActionResult<MessageModel>> SendDemoAsync(SendDemoMessagePayload payload, CancellationToken cancellationToken)
    {
      return Ok(await _messageService.SendDemoAsync(payload, cancellationToken));
    }

    [HttpGet]
    public async Task<ActionResult<ListModel<MessageModel>>> GetAsync(bool? hasErrors, bool? hasSucceeded, bool? isDemo, string? realm, string? search, string? template,
      MessageSort? sort, bool desc,
      int? index, int? count,
      CancellationToken cancellationToken)
    {
      return Ok(await _messageService.GetAsync(hasErrors, hasSucceeded, isDemo, realm, search, template,
        sort, desc,
        index, count,
        cancellationToken));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<MessageModel>> GetAsync(string id, CancellationToken cancellationToken)
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
