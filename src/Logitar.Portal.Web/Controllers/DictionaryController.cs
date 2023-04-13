using Logitar.Portal.Contracts.Dictionaries;
using Logitar.Portal.Web.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Logitar.Portal.Web.Controllers;

[ApiExplorerSettings(IgnoreApi = true)]
[Authorize(Policy = Policies.PortalActor)]
[Route("dictionaries")]
public class DictionaryController : Controller
{
  private readonly IDictionaryService _dictionaryService;

  public DictionaryController(IDictionaryService dictionaryService)
  {
    _dictionaryService = dictionaryService;
  }

  [HttpGet("/create-dictionary")]
  public ActionResult CreateDictionary()
  {
    return View(nameof(DictionaryEdit));
  }

  [HttpGet]
  public ActionResult DictionaryList()
  {
    return View();
  }

  [HttpGet("{id}")]
  public async Task<ActionResult> DictionaryEdit(Guid id, CancellationToken cancellationToken = default)
  {
    Dictionary? dictionary = await _dictionaryService.GetAsync(id, cancellationToken);
    if (dictionary == null)
    {
      return NotFound();
    }

    return View(dictionary);
  }
}
