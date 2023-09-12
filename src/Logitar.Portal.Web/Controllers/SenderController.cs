using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Constants;
using Logitar.Portal.Contracts.Senders;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Logitar.Portal.Web.Controllers;

[ApiController]
[Authorize(Policy = Policies.PortalActor)]
[Route("api/senders")]
public class SenderController : ControllerBase
{
  private readonly ISenderService _senderService;

  public SenderController(ISenderService senderService)
  {
    _senderService = senderService;
  }

  [HttpPost]
  public async Task<ActionResult<Sender>> CreateAsync([FromBody] CreateSenderPayload payload, CancellationToken cancellationToken)
  {
    Sender sender = await _senderService.CreateAsync(payload, cancellationToken);
    Uri uri = new($"{Request.Scheme}://{Request.Host}/api/senders/{sender.Id}");

    return Created(uri, sender);
  }

  [HttpDelete("{id}")]
  public async Task<ActionResult<Sender>> DeleteAsync(Guid id, CancellationToken cancellationToken)
  {
    Sender? sender = await _senderService.DeleteAsync(id, cancellationToken);
    return sender == null ? NotFound() : Ok(sender);
  }

  [HttpGet("{id}")]
  public async Task<ActionResult<Sender>> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    Sender? sender = await _senderService.ReadAsync(id, cancellationToken: cancellationToken);
    return sender == null ? NotFound() : Ok(sender);
  }

  [HttpGet("default")]
  public async Task<ActionResult<Sender>> ReadDefaultAsync(string? realm, CancellationToken cancellationToken)
  {
    Sender? sender = await _senderService.ReadDefaultAsync(realm, cancellationToken: cancellationToken);
    return sender == null ? NotFound() : Ok(sender);
  }

  [HttpPut("{id}")]
  public async Task<ActionResult<Sender>> ReplaceAsync(Guid id, [FromBody] ReplaceSenderPayload payload, long? version, CancellationToken cancellationToken)
  {
    Sender? sender = await _senderService.ReplaceAsync(id, payload, version, cancellationToken);
    return sender == null ? NotFound() : Ok(sender);
  }

  [HttpPost("search")]
  public async Task<ActionResult<SearchResults<Sender>>> SearchAsync([FromBody] SearchSendersPayload payload, CancellationToken cancellationToken)
  {
    return Ok(await _senderService.SearchAsync(payload, cancellationToken));
  }

  [HttpPatch("{id}/default")]
  public async Task<ActionResult<Sender>> SetDefaultAsync(Guid id, CancellationToken cancellationToken)
  {
    Sender? sender = await _senderService.SetDefaultAsync(id, cancellationToken);
    return sender == null ? NotFound() : Ok(sender);
  }

  [HttpPatch("{id}")]
  public async Task<ActionResult<Sender>> UpdateAsync(Guid id, [FromBody] UpdateSenderPayload payload, CancellationToken cancellationToken)
  {
    Sender? sender = await _senderService.UpdateAsync(id, payload, cancellationToken);
    return sender == null ? NotFound() : Ok(sender);
  }
}
