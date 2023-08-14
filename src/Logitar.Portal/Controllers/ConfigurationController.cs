using Logitar.Portal.Application.Configurations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Logitar.Portal.Controllers;

[ApiController]
[Route("configuration")]
public class ConfigurationController : ControllerBase
{
  private readonly IConfigurationService _configurationService;

  public ConfigurationController(IConfigurationService configurationService)
  {
    _configurationService = configurationService;
  }

  [HttpPost]
  public async Task<ActionResult> InitializeAsync([FromBody] InitializeConfigurationPayload payload, CancellationToken cancellationToken)
  {
    await _configurationService.InitializeAsync(payload, cancellationToken);

    // TODO(fpion): sign-in user

    return NoContent();
  }

  [Authorize] // TODO(fpion): PortalIdentity
  [HttpGet]
  public async Task<ActionResult<Configuration>> ReadAsync(CancellationToken cancellationToken)
  {
    return Ok(await _configurationService.ReadAsync(cancellationToken));
  }

  // TODO(fpion): ReplaceAsync
  // TODO(fpion): UpdateAsync
}
