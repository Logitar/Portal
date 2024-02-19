﻿using Logitar.Portal.Contracts.Search;
using Logitar.Portal.Contracts.Templates;
using Logitar.Portal.Extensions;
using Logitar.Portal.Models.Templates;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Logitar.Portal.Controllers;

[ApiController]
[Authorize]
[Route("templates")]
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
    return Created(BuildLocation(template), template);
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
    Template? template = await _templateService.ReadAsync(id: id, cancellationToken: cancellationToken);
    return template == null ? NotFound() : Ok(template);
  }

  [HttpGet("unique-name:{uniqueName}")]
  public async Task<ActionResult<Template>> ReadByUniqueNameAsync(string uniqueName, CancellationToken cancellationToken)
  {
    Template? template = await _templateService.ReadAsync(uniqueName: uniqueName, cancellationToken: cancellationToken);
    return template == null ? NotFound() : Ok(template);
  }

  [HttpPut("{id}")]
  public async Task<ActionResult<Template>> ReplaceAsync(Guid id, [FromBody] ReplaceTemplatePayload payload, long? version, CancellationToken cancellationToken)
  {
    Template? template = await _templateService.ReplaceAsync(id, payload, version, cancellationToken);
    return template == null ? NotFound() : Ok(template);
  }

  [HttpGet]
  public async Task<ActionResult<SearchResults<Template>>> SearchAsync([FromQuery] SearchTemplatesModel model, CancellationToken cancellationToken)
  {
    SearchTemplatesPayload payload = model.ToPayload();
    return Ok(await _templateService.SearchAsync(payload, cancellationToken));
  }

  [HttpPatch("{id}")]
  public async Task<ActionResult<Template>> UpdateAsync(Guid id, [FromBody] UpdateTemplatePayload payload, CancellationToken cancellationToken)
  {
    Template? template = await _templateService.UpdateAsync(id, payload, cancellationToken);
    return template == null ? NotFound() : Ok(template);
  }

  private Uri BuildLocation(Template template) => HttpContext.BuildLocation("templates/{id}", new Dictionary<string, string> { ["id"] = template.Id.ToString() });
}
