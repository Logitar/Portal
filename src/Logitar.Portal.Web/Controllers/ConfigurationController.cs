using Logitar.Portal.Contracts.Configurations;
using Logitar.Portal.Web.Extensions;
using Logitar.Portal.Web.Models.Configuration;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Logitar.Portal.Web.Controllers;

[ApiController]
[Route("api/configuration")]
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
    InitializeConfigurationResult result = await _configurationService.InitializeAsync(payload, cancellationToken);

    HttpContext.SignIn(result.Session);

    return NoContent();
  }

  [HttpGet("initialized")]
  public async Task<ActionResult<IsConfigurationInitializedResult>> IsInitializedAsync(CancellationToken cancellationToken)
  {
    Configuration? configuration = await _configurationService.ReadAsync(cancellationToken);

    return Ok(new IsConfigurationInitializedResult(configuration));
  }

  [Authorize]
  [HttpGet]
  public async Task<ActionResult<Configuration>> ReadAsync(CancellationToken cancellationToken)
  {
    Configuration configuration = await _configurationService.ReadAsync(cancellationToken)
      ?? throw new InvalidOperationException("The configuration has not been initialized.");

    return Ok(configuration);
  }

  [Authorize]
  [HttpPut]
  public async Task<ActionResult<Configuration>> ReplaceAsync([FromBody] ReplaceConfigurationPayload payload, long? version, CancellationToken cancellationToken)
  {
    return Ok(await _configurationService.ReplaceAsync(payload, version, cancellationToken));
  }

  [Authorize]
  [HttpPatch]
  public async Task<ActionResult<Configuration>> UpdateAsync([FromBody] UpdateConfigurationPayload payload, CancellationToken cancellationToken)
  {
    return Ok(await _configurationService.UpdateAsync(payload, cancellationToken));
  }
}
