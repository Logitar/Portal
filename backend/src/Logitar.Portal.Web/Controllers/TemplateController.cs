using Logitar.Portal.Application.Templates;
using Logitar.Portal.Contracts.Search;
using Logitar.Portal.Contracts.Templates;
using Logitar.Portal.Web.Extensions;
using Logitar.Portal.Web.Models.Templates;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Logitar.Portal.Web.Controllers;

[ApiController]
[Authorize]
[Route("api/templates")]
public class TemplateController : ControllerBase
{
  private readonly ITemplateService _templateService;

  public TemplateController(ITemplateService templateService)
  {
    _templateService = templateService;
  }

  [HttpPost]
  public async Task<ActionResult<TemplateModel>> CreateAsync([FromBody] CreateTemplatePayload payload, CancellationToken cancellationToken)
  {
    TemplateModel template = await _templateService.CreateAsync(payload, cancellationToken);
    return Created(BuildLocation(template), template);
  }

  [HttpDelete("{id}")]
  public async Task<ActionResult<TemplateModel>> DeleteAsync(Guid id, CancellationToken cancellationToken)
  {
    TemplateModel? template = await _templateService.DeleteAsync(id, cancellationToken);
    return template == null ? NotFound() : Ok(template);
  }

  [HttpGet("{id}")]
  public async Task<ActionResult<TemplateModel>> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    TemplateModel? template = await _templateService.ReadAsync(id: id, cancellationToken: cancellationToken);
    return template == null ? NotFound() : Ok(template);
  }

  [HttpGet("unique-key:{uniqueKey}")]
  public async Task<ActionResult<TemplateModel>> ReadByUniqueKeyAsync(string uniqueKey, CancellationToken cancellationToken)
  {
    TemplateModel? template = await _templateService.ReadAsync(uniqueKey: uniqueKey, cancellationToken: cancellationToken);
    return template == null ? NotFound() : Ok(template);
  }

  [HttpPut("{id}")]
  public async Task<ActionResult<TemplateModel>> ReplaceAsync(Guid id, [FromBody] ReplaceTemplatePayload payload, long? version, CancellationToken cancellationToken)
  {
    TemplateModel? template = await _templateService.ReplaceAsync(id, payload, version, cancellationToken);
    return template == null ? NotFound() : Ok(template);
  }

  [HttpGet]
  public async Task<ActionResult<SearchResults<TemplateModel>>> SearchAsync([FromQuery] SearchTemplatesModel model, CancellationToken cancellationToken)
  {
    SearchTemplatesPayload payload = model.ToPayload();
    return Ok(await _templateService.SearchAsync(payload, cancellationToken));
  }

  [HttpPatch("{id}")]
  public async Task<ActionResult<TemplateModel>> UpdateAsync(Guid id, [FromBody] UpdateTemplatePayload payload, CancellationToken cancellationToken)
  {
    TemplateModel? template = await _templateService.UpdateAsync(id, payload, cancellationToken);
    return template == null ? NotFound() : Ok(template);
  }

  private Uri BuildLocation(TemplateModel template) => HttpContext.BuildLocation("templates/{id}", new Dictionary<string, string> { ["id"] = template.Id.ToString() });
}
