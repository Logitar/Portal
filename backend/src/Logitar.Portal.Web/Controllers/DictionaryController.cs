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
  public async Task<ActionResult<DictionaryModel>> CreateAsync([FromBody] CreateDictionaryPayload payload, CancellationToken cancellationToken)
  {
    DictionaryModel dictionary = await _dictionaryService.CreateAsync(payload, cancellationToken);
    return Created(BuildLocation(dictionary), dictionary);
  }

  [HttpDelete("{id}")]
  public async Task<ActionResult<DictionaryModel>> DeleteAsync(Guid id, CancellationToken cancellationToken)
  {
    DictionaryModel? dictionary = await _dictionaryService.DeleteAsync(id, cancellationToken);
    return dictionary == null ? NotFound() : Ok(dictionary);
  }

  [HttpGet("{id}")]
  public async Task<ActionResult<DictionaryModel>> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    DictionaryModel? dictionary = await _dictionaryService.ReadAsync(id: id, cancellationToken: cancellationToken);
    return dictionary == null ? NotFound() : Ok(dictionary);
  }

  [HttpGet("locale:{locale}")]
  public async Task<ActionResult<DictionaryModel>> ReadByLocaleAsync(string locale, CancellationToken cancellationToken)
  {
    DictionaryModel? dictionary = await _dictionaryService.ReadAsync(locale: locale, cancellationToken: cancellationToken);
    return dictionary == null ? NotFound() : Ok(dictionary);
  }

  [HttpPut("{id}")]
  public async Task<ActionResult<DictionaryModel>> ReplaceAsync(Guid id, [FromBody] ReplaceDictionaryPayload payload, long? version, CancellationToken cancellationToken)
  {
    DictionaryModel? dictionary = await _dictionaryService.ReplaceAsync(id, payload, version, cancellationToken);
    return dictionary == null ? NotFound() : Ok(dictionary);
  }

  [HttpGet]
  public async Task<ActionResult<SearchResults<DictionaryModel>>> SearchAsync([FromQuery] SearchDictionariesModel model, CancellationToken cancellationToken)
  {
    SearchDictionariesPayload payload = model.ToPayload();
    return Ok(await _dictionaryService.SearchAsync(payload, cancellationToken));
  }

  [HttpPatch("{id}")]
  public async Task<ActionResult<DictionaryModel>> UpdateAsync(Guid id, [FromBody] UpdateDictionaryPayload payload, CancellationToken cancellationToken)
  {
    DictionaryModel? dictionary = await _dictionaryService.UpdateAsync(id, payload, cancellationToken);
    return dictionary == null ? NotFound() : Ok(dictionary);
  }

  private Uri BuildLocation(DictionaryModel dictionary) => HttpContext.BuildLocation("dictionaries/{id}", new Dictionary<string, string> { ["id"] = dictionary.Id.ToString() });
}
