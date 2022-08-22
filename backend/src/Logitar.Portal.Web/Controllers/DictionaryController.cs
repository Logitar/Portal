using Logitar.Portal.Application.Dictionaries;
using Logitar.Portal.Core.Dictionaries.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Logitar.Portal.Web.Controllers
{
  [ApiExplorerSettings(IgnoreApi = true)]
  [Authorize(Policy = Constants.Policies.PortalIdentity)]
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
      DictionaryModel? dictionary = await _dictionaryService.GetAsync(id, cancellationToken);
      if (dictionary == null)
      {
        return NotFound();
      }

      return View(dictionary);
    }
  }
}
