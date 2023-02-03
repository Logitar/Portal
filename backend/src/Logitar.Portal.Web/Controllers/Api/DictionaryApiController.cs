using Logitar.Portal.Application.Dictionaries;
using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Dictionaries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;

namespace Logitar.Portal.Web.Controllers.Api
{
  [ApiController]
  [Authorize(Policy = Constants.Policies.PortalIdentity)]
  [Route("api/dictionaries")]
  public class DictionaryApiController : ControllerBase
  {
    private readonly IDictionaryService _dictionaryService;

    public DictionaryApiController(IDictionaryService dictionaryService)
    {
      _dictionaryService = dictionaryService;
    }

    [HttpPost]
    public async Task<ActionResult<DictionaryModel>> CreateAsync([FromBody] CreateDictionaryPayload payload, CancellationToken cancellationToken)
    {
      DictionaryModel dictionary = await _dictionaryService.CreateAsync(payload, cancellationToken);
      var uri = new Uri($"/api/dictionaries/{dictionary.Id}", UriKind.Relative);

      return Created(uri, dictionary);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<DictionaryModel>> DeleteAsync(string id, CancellationToken cancellationToken)
    {
      await _dictionaryService.DeleteAsync(id, cancellationToken);

      return NoContent();
    }

    [HttpGet]
    public async Task<ActionResult<ListModel<DictionaryModel>>> GetAsync(CultureInfo? locale, string? realm,
      DictionarySort? sort, bool desc,
      int? index, int? count,
      CancellationToken cancellationToken = default)
    {
      return Ok(await _dictionaryService.GetAsync(locale, realm,
        sort, desc,
        index, count,
        cancellationToken));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<DictionaryModel>> GetAsync(string id, CancellationToken cancellationToken)
    {
      DictionaryModel? dictionary = await _dictionaryService.GetAsync(id, cancellationToken);
      if (dictionary == null)
      {
        return NotFound();
      }

      return Ok(dictionary);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<DictionaryModel>> UpdateAsync(string id, [FromBody] UpdateDictionaryPayload payload, CancellationToken cancellationToken)
    {
      return Ok(await _dictionaryService.UpdateAsync(id, payload, cancellationToken));
    }
  }
}
