using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Portal.Core;
using Portal.Core.Emails.Templates;
using Portal.Core.Emails.Templates.Models;
using Portal.Core.Emails.Templates.Payloads;

namespace Portal.Web.Controllers.Api
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
      var uri = new Uri($"/api/templates/{template.Id}", UriKind.Relative);

      return Created(uri, template);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<TemplateModel>> DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
      return Ok(await _templateService.DeleteAsync(id, cancellationToken));
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
    public async Task<ActionResult<TemplateModel>> GetAsync(Guid id, CancellationToken cancellationToken)
    {
      TemplateModel? template = await _templateService.GetAsync(id, cancellationToken);
      if (template == null)
      {
        return NotFound();
      }

      return Ok(template);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<TemplateModel>> UpdateAsync(Guid id, [FromBody] UpdateTemplatePayload payload, CancellationToken cancellationToken)
    {
      return Ok(await _templateService.UpdateAsync(id, payload, cancellationToken));
    }
  }
}
