using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Constants;
using Logitar.Portal.Contracts.Roles;
using Logitar.Portal.Contracts.Templates;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Logitar.Portal.Web.Controllers;

[ApiController]
[Authorize(Policy = Policies.PortalActor)]
[Route("api/templates")]
public class TemplateController : ControllerBase
{
  private readonly ITemplateService _templateService;

  public TemplateController(ITemplateService templateService)
  {
    _templateService = templateService;
  }

  [HttpPost]
  public async Task<ActionResult<Template>> CreateAsync([FromBody] CreateTemplatePayload payload, CancellationToken cancellationToken)
  {
    Template template = await _templateService.CreateAsync(payload, cancellationToken);
    Uri uri = new($"{Request.Scheme}://{Request.Host}/api/templates/{template.Id}");

    return Created(uri, template);
  }

  [HttpDelete("{id}")]
  public async Task<ActionResult<Template>> DeleteAsync(Guid id, CancellationToken cancellationToken)
  {
    Template? template = await _templateService.DeleteAsync(id, cancellationToken);
    return template == null ? NotFound() : Ok(template);
  }

  [HttpGet("{id}")]
  public async Task<ActionResult<Template>> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    Template? template = await _templateService.ReadAsync(id, cancellationToken: cancellationToken);
    return template == null ? NotFound() : Ok(template);
  }

  [HttpGet("unique-name:{uniqueName}")]
  public async Task<ActionResult<Role>> ReadAsync(string uniqueName, string? realm, CancellationToken cancellationToken)
  {
    Template? template = await _templateService.ReadAsync(realm: realm, uniqueName: uniqueName, cancellationToken: cancellationToken);
    return template == null ? NotFound() : Ok(template);
  }

  [HttpPut("{id}")]
  public async Task<ActionResult<Template>> ReplaceAsync(Guid id, [FromBody] ReplaceTemplatePayload payload, long? version, CancellationToken cancellationToken)
  {
    Template? template = await _templateService.ReplaceAsync(id, payload, version, cancellationToken);
    return template == null ? NotFound() : Ok(template);
  }

  [HttpPost("search")]
  public async Task<ActionResult<SearchResults<Template>>> SearchAsync([FromBody] SearchTemplatesPayload payload, CancellationToken cancellationToken)
  {
    return Ok(await _templateService.SearchAsync(payload, cancellationToken));
  }

  [HttpPatch("{id}")]
  public async Task<ActionResult<Template>> UpdateAsync(Guid id, [FromBody] UpdateTemplatePayload payload, CancellationToken cancellationToken)
  {
    Template? template = await _templateService.UpdateAsync(id, payload, cancellationToken);
    return template == null ? NotFound() : Ok(template);
  }
}
