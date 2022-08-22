using AutoMapper;
using Logitar.Portal.Application.Dictionaries;
using Logitar.Portal.Core;
using Logitar.Portal.Core.Dictionaries;
using Logitar.Portal.Core.Dictionaries.Models;
using Logitar.Portal.Core.Dictionaries.Payloads;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Logitar.Portal.Web.Controllers.Api
{
  [ApiController]
  [Authorize(Policy = Constants.Policies.PortalIdentity)]
  [Route("api/dictionaries")]
  public class DictionaryApiController : ControllerBase
  {
    private readonly IMapper _mapper;
    private readonly IDictionaryService _dictionaryService;

    public DictionaryApiController(IMapper mapper, IDictionaryService dictionaryService)
    {
      _mapper = mapper;
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
    public async Task<ActionResult<DictionaryModel>> DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
      return Ok(await _dictionaryService.DeleteAsync(id, cancellationToken));
    }

    [HttpGet]
    public async Task<ActionResult<ListModel<DictionarySummary>>> GetAsync(string? locale, string? realm,
      DictionarySort? sort, bool desc,
      int? index, int? count,
      CancellationToken cancellationToken = default)
    {
      ListModel<DictionaryModel> dictionaries = await _dictionaryService.GetAsync(locale, realm,
        sort, desc,
        index, count,
        cancellationToken);

      return Ok(dictionaries.To<DictionaryModel, DictionarySummary>(_mapper));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<DictionaryModel>> GetAsync(Guid id, CancellationToken cancellationToken)
    {
      DictionaryModel? dictionary = await _dictionaryService.GetAsync(id, cancellationToken);
      if (dictionary == null)
      {
        return NotFound();
      }

      return Ok(dictionary);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<DictionaryModel>> UpdateAsync(Guid id, [FromBody] UpdateDictionaryPayload payload, CancellationToken cancellationToken)
    {
      return Ok(await _dictionaryService.UpdateAsync(id, payload, cancellationToken));
    }
  }
}
