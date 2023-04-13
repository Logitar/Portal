﻿using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Senders;
using Logitar.Portal.Web.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Logitar.Portal.Web.Controllers.Api;

[ApiController]
[Authorize(Policy = Policies.PortalActor)]
[Route("api/senders")]
public class SenderApiController : ControllerBase
{
  private readonly ISenderService _senderService;

  public SenderApiController(ISenderService senderService)
  {
    _senderService = senderService;
  }

  [HttpPost]
  public async Task<ActionResult<Sender>> CreateAsync([FromBody] CreateSenderInput input, CancellationToken cancellationToken)
  {
    Sender sender = await _senderService.CreateAsync(input, cancellationToken);
    Uri uri = new($"{Request.Scheme}://{Request.Host}/api/senders/{sender.Id}");

    return Created(uri, sender);
  }

  [HttpDelete("{id}")]
  public async Task<ActionResult<Sender>> DeleteAsync(Guid id, CancellationToken cancellationToken)
  {
    return Ok(await _senderService.DeleteAsync(id, cancellationToken));
  }

  [HttpGet]
  public async Task<ActionResult<PagedList<Sender>>> GetAsync(ProviderType? provider, string? realm, string? search,
    SenderSort? sort, bool isDescending, int? skip, int? limit, CancellationToken cancellationToken = default)
  {
    return Ok(await _senderService.GetAsync(provider, realm, search,
      sort, isDescending, skip, limit, cancellationToken));
  }

  [HttpGet("default/{realm}")]
  public async Task<ActionResult<Sender>> GetDefaultAsync(string realm, CancellationToken cancellationToken)
  {
    Sender? sender = await _senderService.GetAsync(defaultInRealm: realm, cancellationToken: cancellationToken);
    if (sender == null)
    {
      return NotFound();
    }

    return Ok(sender);
  }

  [HttpGet("{id}")]
  public async Task<ActionResult<Sender>> GetAsync(Guid id, CancellationToken cancellationToken)
  {
    Sender? sender = await _senderService.GetAsync(id, cancellationToken: cancellationToken);
    if (sender == null)
    {
      return NotFound();
    }

    return Ok(sender);
  }

  [HttpPut("{id}")]
  public async Task<ActionResult<Sender>> UpdateAsync(Guid id, [FromBody] UpdateSenderInput input, CancellationToken cancellationToken)
  {
    return Ok(await _senderService.UpdateAsync(id, input, cancellationToken));
  }

  [HttpPatch("{id}/default")]
  public async Task<ActionResult<Sender>> SetDefaultAsync(Guid id, CancellationToken cancellationToken)
  {
    return Ok(await _senderService.SetDefaultAsync(id, cancellationToken));
  }
}
