using Logitar.Portal.Contracts.Templates;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Logitar.Portal.Web.Controllers;

[ApiExplorerSettings(IgnoreApi = true)]
[Authorize(Policy = Constants.Policies.PortalActor)]
[Route("templates")]
public class TemplateController : Controller
{
  private readonly ITemplateService _templateService;

  public TemplateController(ITemplateService templateService)
  {
    _templateService = templateService;
  }

  [HttpGet("/create-template")]
  public ActionResult CreateTemplate()
  {
    return View(nameof(TemplateEdit));
  }

  [HttpGet]
  public ActionResult TemplateList()
  {
    return View();
  }

  [HttpGet("{id}")]
  public async Task<ActionResult> TemplateEdit(Guid id, CancellationToken cancellationToken = default)
  {
    Template? template = await _templateService.GetAsync(id, cancellationToken: cancellationToken);
    if (template == null)
    {
      return NotFound();
    }

    return View(template);
  }
}
