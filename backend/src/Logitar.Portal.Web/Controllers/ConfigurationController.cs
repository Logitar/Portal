using Logitar.Portal.Application.Configurations;
using Logitar.Portal.Contracts.Configurations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Logitar.Portal.Web.Controllers;

[ApiController]
[Authorize]
[Route("api/configuration")]
public class ConfigurationController : ControllerBase
{
  private readonly IConfigurationService _configurationService;

  public ConfigurationController(IConfigurationService configurationService)
  {
    _configurationService = configurationService;
  }

  [HttpGet]
  public async Task<ActionResult<Configuration>> ReadAsync(CancellationToken cancellationToken)
  {
    return Ok(await _configurationService.ReadAsync(cancellationToken));
  }

  [HttpPut]
  public async Task<ActionResult<Configuration>> ReplaceAsync([FromBody] ReplaceConfigurationPayload payload, long? version, CancellationToken cancellationToken)
  {
    return Ok(await _configurationService.ReplaceAsync(payload, version, cancellationToken));
  }

  [HttpPatch]
  public async Task<ActionResult<Configuration>> UpdateAsync([FromBody] UpdateConfigurationPayload payload, CancellationToken cancellationToken)
  {
    return Ok(await _configurationService.UpdateAsync(payload, cancellationToken));
  }
}
