using Logitar.Portal.v2.Contracts;
using Logitar.Portal.v2.Contracts.Messages;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Logitar.Portal.v2.Web.Controllers.Api;

[ApiController]
[Authorize(Policy = Constants.Policies.PortalActor)]
[Route("api/messages")]
public class MessageApiController : ControllerBase
{
  private readonly IMessageService _messageService;

  public MessageApiController(IMessageService messageService)
  {
    _messageService = messageService;
  }

  [HttpGet]
  public async Task<ActionResult<PagedList<Message>>> GetAsync(bool? hasErrors, bool? isDemo, string? realm, string? search, bool? succeeded, string? template,
    MessageSort? sort, bool isDescending, int? skip, int? limit, CancellationToken cancellationToken)
  {
    return Ok(await _messageService.GetAsync(hasErrors, isDemo, realm, search, succeeded, template,
      sort, isDescending, skip, limit, cancellationToken));
  }

  [HttpGet("{id}")]
  public async Task<ActionResult<Message>> GetAsync(Guid id, CancellationToken cancellationToken)
  {
    Message? message = await _messageService.GetAsync(id, cancellationToken);
    if (message == null)
    {
      return NotFound();
    }

    return Ok(message);
  }

  [HttpPost]
  public async Task<ActionResult<SentMessages>> SendAsync(SendMessageInput input, CancellationToken cancellationToken)
  {
    return Ok(await _messageService.SendAsync(input, cancellationToken));
  }

  [HttpPost("demo")]
  [Authorize(Policy = Constants.Policies.AuthenticatedPortalUser)]
  public async Task<ActionResult<Message>> SendDemoAsync(SendDemoMessageInput input, CancellationToken cancellationToken)
  {
    return Ok(await _messageService.SendDemoAsync(input, cancellationToken));
  }
}
