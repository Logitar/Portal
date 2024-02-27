using Logitar.Portal.Application.Dictionaries;
using Logitar.Portal.Contracts.Dictionaries;
using Logitar.Portal.Contracts.Search;
using Logitar.Portal.Web.Extensions;
using Logitar.Portal.Web.Models.Dictionaries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Logitar.Portal.Web.Controllers;

[ApiController]
[Authorize]
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
    return Created(BuildLocation(dictionary), dictionary);
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
    Dictionary? dictionary = await _dictionaryService.ReadAsync(id: id, cancellationToken: cancellationToken);
    return dictionary == null ? NotFound() : Ok(dictionary);
  }

  [HttpGet("locale:{locale}")]
  public async Task<ActionResult<Dictionary>> ReadByLocaleAsync(string locale, CancellationToken cancellationToken)
  {
    Dictionary? dictionary = await _dictionaryService.ReadAsync(locale: locale, cancellationToken: cancellationToken);
    return dictionary == null ? NotFound() : Ok(dictionary);
  }

  [HttpPut("{id}")]
  public async Task<ActionResult<Dictionary>> ReplaceAsync(Guid id, [FromBody] ReplaceDictionaryPayload payload, long? version, CancellationToken cancellationToken)
  {
    Dictionary? dictionary = await _dictionaryService.ReplaceAsync(id, payload, version, cancellationToken);
    return dictionary == null ? NotFound() : Ok(dictionary);
  }

  [HttpGet]
  public async Task<ActionResult<SearchResults<Dictionary>>> SearchAsync([FromQuery] SearchDictionariesModel model, CancellationToken cancellationToken)
  {
    SearchDictionariesPayload payload = model.ToPayload();
    return Ok(await _dictionaryService.SearchAsync(payload, cancellationToken));
  }

  [HttpPatch("{id}")]
  public async Task<ActionResult<Dictionary>> UpdateAsync(Guid id, [FromBody] UpdateDictionaryPayload payload, CancellationToken cancellationToken)
  {
    Dictionary? dictionary = await _dictionaryService.UpdateAsync(id, payload, cancellationToken);
    return dictionary == null ? NotFound() : Ok(dictionary);
  }

  private Uri BuildLocation(Dictionary dictionary) => HttpContext.BuildLocation("dictionaries/{id}", new Dictionary<string, string> { ["id"] = dictionary.Id.ToString() });
}
