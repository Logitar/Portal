using Logitar.Portal.Application.Templates;
using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Templates;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Logitar.Portal.Web.Controllers.Api
{
  [ApiController]
  [Authorize(Policy = Constants.Policies.PortalIdentity)]
  [Route("api/templates")]
  public class TemplateApiController : ControllerBase
  {
    private readonly ITemplateService _templateService;

    public TemplateApiController(ITemplateService templateService)
    {
      _templateService = templateService;
    }

    [HttpPost]
    public async Task<ActionResult<TemplateModel>> CreateAsync([FromBody] CreateTemplatePayload payload, CancellationToken cancellationToken)
    {
      TemplateModel template = await _templateService.CreateAsync(payload, cancellationToken);
      Uri uri = new($"/api/templates/{template.Id}", UriKind.Relative);

      return Created(uri, template);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<TemplateModel>> DeleteAsync(string id, CancellationToken cancellationToken)
    {
      await _templateService.DeleteAsync(id, cancellationToken);

      return NoContent();
    }

    [HttpGet]
    public async Task<ActionResult<ListModel<TemplateModel>>> GetAsync(string? realm, string? search,
      TemplateSort? sort, bool desc,
      int? index, int? count,
      CancellationToken cancellationToken = default)
    {
      return Ok(await _templateService.GetAsync(realm, search,
        sort, desc,
        index, count,
        cancellationToken));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<TemplateModel>> GetAsync(string id, CancellationToken cancellationToken)
    {
      TemplateModel? template = await _templateService.GetAsync(id, cancellationToken);
      if (template == null)
      {
        return NotFound();
      }

      return Ok(template);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<TemplateModel>> UpdateAsync(string id, [FromBody] UpdateTemplatePayload payload, CancellationToken cancellationToken)
    {
      return Ok(await _templateService.UpdateAsync(id, payload, cancellationToken));
    }
  }
}
