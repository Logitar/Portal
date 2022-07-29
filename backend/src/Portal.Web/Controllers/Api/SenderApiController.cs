using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Portal.Core;
using Portal.Core.Senders;
using Portal.Core.Senders.Models;
using Portal.Core.Senders.Payloads;

namespace Portal.Web.Controllers.Api
{
  [ApiController]
  [Authorize(Policy = Constants.Policies.PortalIdentity)]
  [Route("api/senders")]
  public class SenderApiController : ControllerBase
  {
    private readonly ISenderService _senderService;

    public SenderApiController(ISenderService senderService)
    {
      _senderService = senderService;
    }

    [HttpPost]
    public async Task<ActionResult<SenderModel>> CreateAsync([FromBody] CreateSenderPayload payload, CancellationToken cancellationToken)
    {
      SenderModel sender = await _senderService.CreateAsync(payload, cancellationToken);
      var uri = new Uri($"/api/senders/{sender.Id}", UriKind.Relative);

      return Created(uri, sender);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<SenderModel>> DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
      return Ok(await _senderService.DeleteAsync(id, cancellationToken));
    }

    [HttpGet]
    public async Task<ActionResult<ListModel<SenderModel>>> GetAsync(ProviderType? provider, Guid? realmId, string? search,
      SenderSort? sort, bool desc,
      int? index, int? count,
      CancellationToken cancellationToken = default)
    {
      return Ok(await _senderService.GetAsync(provider, realmId, search,
        sort, desc,
        index, count,
        cancellationToken));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<SenderModel>> GetAsync(Guid id, CancellationToken cancellationToken)
    {
      SenderModel? sender = await _senderService.GetAsync(id, cancellationToken);
      if (sender == null)
      {
        return NotFound();
      }

      return Ok(sender);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<SenderModel>> UpdateAsync(Guid id, [FromBody] UpdateSenderPayload payload, CancellationToken cancellationToken)
    {
      return Ok(await _senderService.UpdateAsync(id, payload, cancellationToken));
    }

    [HttpPatch("{id}/default")]
    public async Task<ActionResult<SenderModel>> SetDefaultAsync(Guid id, CancellationToken cancellationToken)
    {
      return Ok(await _senderService.SetDefaultAsync(id, cancellationToken));
    }
  }
}
