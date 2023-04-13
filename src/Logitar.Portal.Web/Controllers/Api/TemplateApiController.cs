using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Templates;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Logitar.Portal.Web.Controllers.Api;

[ApiController]
[Authorize(Policy = Constants.Policies.PortalActor)]
[Route("api/templates")]
public class TemplateApiController : ControllerBase
{
  private readonly ITemplateService _templateService;

  public TemplateApiController(ITemplateService templateService)
  {
    _templateService = templateService;
  }

  [HttpPost]
  public async Task<ActionResult<Template>> CreateAsync([FromBody] CreateTemplateInput input, CancellationToken cancellationToken)
  {
    Template template = await _templateService.CreateAsync(input, cancellationToken);
    Uri uri = new($"{Request.Scheme}://{Request.Host}/api/templates/{template.Id}");

    return Created(uri, template);
  }

  [HttpDelete("{id}")]
  public async Task<ActionResult<Template>> DeleteAsync(Guid id, CancellationToken cancellationToken)
  {
    return Ok(await _templateService.DeleteAsync(id, cancellationToken));
  }

  [HttpGet]
  public async Task<ActionResult<PagedList<Template>>> GetAsync(string? realm, string? search,
    TemplateSort? sort, bool isDescending, int? skip, int? limit, CancellationToken cancellationToken)
  {
    return Ok(await _templateService.GetAsync(realm, search,
      sort, isDescending, skip, limit, cancellationToken));
  }

  [HttpGet("{id}")]
  public async Task<ActionResult<Template>> GetAsync(Guid id, CancellationToken cancellationToken)
  {
    Template? template = await _templateService.GetAsync(id, cancellationToken: cancellationToken);
    if (template == null)
    {
      return NotFound();
    }

    return Ok(template);
  }

  [HttpPut("{id}")]
  public async Task<ActionResult<Template>> UpdateAsync(Guid id, [FromBody] UpdateTemplateInput input, CancellationToken cancellationToken)
  {
    return Ok(await _templateService.UpdateAsync(id, input, cancellationToken));
  }
}
