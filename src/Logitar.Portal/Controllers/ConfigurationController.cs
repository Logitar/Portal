using Logitar.Portal.Contracts.Configurations;
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

  [Authorize] // TODO(fpion): PortalIdentity
  [HttpPut]
  public async Task<ActionResult<Configuration>> ReplaceAsync([FromBody] ReplaceConfigurationPayload payload, long? version, CancellationToken cancellationToken)
  {
    return Ok(await _configurationService.ReplaceAsync(payload, version, cancellationToken));
  }

  [Authorize] // TODO(fpion): PortalIdentity
  [HttpPatch]
  public async Task<ActionResult<Configuration>> UpdateAsync([FromBody] UpdateConfigurationPayload payload, CancellationToken cancellationToken)
  {
    return Ok(await _configurationService.UpdateAsync(payload, cancellationToken));
  }
}
