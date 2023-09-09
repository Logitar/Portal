using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Constants;
using Logitar.Portal.Contracts.Dictionaries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Logitar.Portal.Web.Controllers;

[ApiController]
[Authorize(Policy = Policies.PortalActor)]
[Route("api/dictionaries")]
public class DictionaryController : ControllerBase
{
  private readonly IDictionaryService _dictionaryService;

  public DictionaryController(IDictionaryService dictionaryService)
  {
    _dictionaryService = dictionaryService;
  }

  [HttpPost]
  public async Task<ActionResult<Dictionary>> CreateAsync([FromBody] CreateDictionaryPayload payload, CancellationToken cancellationToken)
  {
    Dictionary dictionary = await _dictionaryService.CreateAsync(payload, cancellationToken);
    Uri uri = new($"{Request.Scheme}://{Request.Host}/api/dictionaries/{dictionary.Id}");

    return Created(uri, dictionary);
  }

  [HttpDelete("{id}")]
  public async Task<ActionResult<Dictionary>> DeleteAsync(Guid id, CancellationToken cancellationToken)
  {
    Dictionary? dictionary = await _dictionaryService.DeleteAsync(id, cancellationToken);
    return dictionary == null ? NotFound() : Ok(dictionary);
  }

  [HttpGet("{id}")]
  public async Task<ActionResult<Dictionary>> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    Dictionary? dictionary = await _dictionaryService.ReadAsync(id, cancellationToken: cancellationToken);
    return dictionary == null ? NotFound() : Ok(dictionary);
  }

  [HttpGet("locale:{locale}")]
  public async Task<ActionResult<Dictionary>> ReadAsync(string locale, string? realm, CancellationToken cancellationToken)
  {
    Dictionary? dictionary = await _dictionaryService.ReadAsync(realm: realm, locale: locale, cancellationToken: cancellationToken);
    return dictionary == null ? NotFound() : Ok(dictionary);
  }

  [HttpPut("{id}")]
  public async Task<ActionResult<Dictionary>> ReplaceAsync(Guid id, [FromBody] ReplaceDictionaryPayload payload, long? version, CancellationToken cancellationToken)
  {
    Dictionary? dictionary = await _dictionaryService.ReplaceAsync(id, payload, version, cancellationToken);
    return dictionary == null ? NotFound() : Ok(dictionary);
  }

  [HttpPost("search")]
  public async Task<ActionResult<SearchResults<Dictionary>>> SearchAsync([FromBody] SearchDictionariesPayload payload, CancellationToken cancellationToken)
  {
    return Ok(await _dictionaryService.SearchAsync(payload, cancellationToken));
  }

  [HttpPatch("{id}")]
  public async Task<ActionResult<Dictionary>> UpdateAsync(Guid id, [FromBody] UpdateDictionaryPayload payload, CancellationToken cancellationToken)
  {
    Dictionary? dictionary = await _dictionaryService.UpdateAsync(id, payload, cancellationToken);
    return dictionary == null ? NotFound() : Ok(dictionary);
  }
}
