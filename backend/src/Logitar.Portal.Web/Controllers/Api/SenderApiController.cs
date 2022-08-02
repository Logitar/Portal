using AutoMapper;
using Logitar.Portal.Core;
using Logitar.Portal.Core.Emails.Senders;
using Logitar.Portal.Core.Emails.Senders.Models;
using Logitar.Portal.Core.Emails.Senders.Payloads;
using Logitar.Portal.Web.Models.Api.Sender;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Logitar.Portal.Web.Controllers.Api
{
  [ApiController]
  [Authorize(Policy = Constants.Policies.PortalIdentity)]
  [Route("api/senders")]
  public class SenderApiController : ControllerBase
  {
    private readonly IMapper _mapper;
    private readonly ISenderService _senderService;

    public SenderApiController(IMapper mapper, ISenderService senderService)
    {
      _mapper = mapper;
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
    public async Task<ActionResult<ListModel<SenderSummary>>> GetAsync(ProviderType? provider, string? realm, string? search,
      SenderSort? sort, bool desc,
      int? index, int? count,
      CancellationToken cancellationToken = default)
    {
      ListModel<SenderModel> senders = await _senderService.GetAsync(provider, realm, search,
        sort, desc,
        index, count,
        cancellationToken);

      return Ok(ListModel<SenderSummary>.From(senders, _mapper));
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
