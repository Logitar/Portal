using Logitar.Portal.Application.Senders;
using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Senders;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Logitar.Portal.Web.Controllers.Api
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
      Uri uri = new($"/api/senders/{sender.Id}", UriKind.Relative);

      return Created(uri, sender);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<SenderModel>> DeleteAsync(string id, CancellationToken cancellationToken)
    {
      await _senderService.DeleteAsync(id, cancellationToken);

      return Ok();
    }

    [HttpGet]
    public async Task<ActionResult<ListModel<SenderModel>>> GetAsync(ProviderType? provider, string? realm, string? search,
      SenderSort? sort, bool desc,
      int? index, int? count,
      CancellationToken cancellationToken = default)
    {
      return Ok(await _senderService.GetAsync(provider, realm, search,
        sort, desc,
        index, count,
        cancellationToken));
    }

    [HttpGet("default")]
    public async Task<ActionResult<SenderModel>> GetDefaultAsync(string? realm, CancellationToken cancellationToken)
    {
      SenderModel? sender = await _senderService.GetDefaultAsync(realm, cancellationToken);
      if (sender == null)
      {
        return NotFound();
      }

      return Ok(sender);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<SenderModel>> GetAsync(string id, CancellationToken cancellationToken)
    {
      SenderModel? sender = await _senderService.GetAsync(id, cancellationToken);
      if (sender == null)
      {
        return NotFound();
      }

      return Ok(sender);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<SenderModel>> UpdateAsync(string id, [FromBody] UpdateSenderPayload payload, CancellationToken cancellationToken)
    {
      return Ok(await _senderService.UpdateAsync(id, payload, cancellationToken));
    }

    [HttpPatch("{id}/default")]
    public async Task<ActionResult<SenderModel>> SetDefaultAsync(string id, CancellationToken cancellationToken)
    {
      return Ok(await _senderService.SetDefaultAsync(id, cancellationToken));
    }
  }
}
