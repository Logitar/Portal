﻿using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Dictionaries;
using Logitar.Portal.Web.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Logitar.Portal.Web.Controllers.Api;

[ApiController]
[Authorize(Policy = Policies.PortalActor)]
[Route("api/dictionaries")]
public class DictionaryApiController : ControllerBase
{
  private readonly IDictionaryService _dictionaryService;

  public DictionaryApiController(IDictionaryService dictionaryService)
  {
    _dictionaryService = dictionaryService;
  }

  [HttpPost]
  public async Task<ActionResult<Dictionary>> CreateAsync([FromBody] CreateDictionaryInput input, CancellationToken cancellationToken)
  {
    Dictionary dictionary = await _dictionaryService.CreateAsync(input, cancellationToken);
    Uri uri = new($"{Request.Scheme}://{Request.Host}/api/dictionaries/{dictionary.Id}");

    return Created(uri, dictionary);
  }

  [HttpDelete("{id}")]
  public async Task<ActionResult<Dictionary>> DeleteAsync(Guid id, CancellationToken cancellationToken)
  {
    return Ok(await _dictionaryService.DeleteAsync(id, cancellationToken));
  }

  [HttpGet]
  public async Task<ActionResult<PagedList<Dictionary>>> GetAsync(string? locale, string? realm,
    DictionarySort? sort, bool isDescending, int? skip, int? limit, CancellationToken cancellationToken)
  {
    return Ok(await _dictionaryService.GetAsync(locale, realm, sort, isDescending, skip, limit, cancellationToken));
  }

  [HttpGet("{id}")]
  public async Task<ActionResult<Dictionary>> GetAsync(Guid id, CancellationToken cancellationToken)
  {
    Dictionary? dictionary = await _dictionaryService.GetAsync(id, cancellationToken);
    if (dictionary == null)
    {
      return NotFound();
    }

    return Ok(dictionary);
  }

  [HttpPut("{id}")]
  public async Task<ActionResult<Dictionary>> UpdateAsync(Guid id, [FromBody] UpdateDictionaryInput input, CancellationToken cancellationToken)
  {
    return Ok(await _dictionaryService.UpdateAsync(id, input, cancellationToken));
  }
}
