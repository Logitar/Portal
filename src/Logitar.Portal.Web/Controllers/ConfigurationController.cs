using Logitar.Portal.Contracts.Configurations;
using Logitar.Portal.Contracts.Sessions;
using Logitar.Portal.Web.Constants;
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
  public async Task<ActionResult> InitializeAsync([FromBody] Models.Configuration.InitializeConfigurationPayload payload, CancellationToken cancellationToken)
  {
    Contracts.Configurations.InitializeConfigurationPayload initializePayload = new()
    {
      Locale = payload.Locale,
      User = payload.User,
      Session = new SessionPayload
      {
        CustomAttributes = HttpContext.GetSessionCustomAttributes()
      }
    };
    Session session = await _configurationService.InitializeAsync(initializePayload, cancellationToken);

    HttpContext.SignIn(session);

    return NoContent();
  }

  [HttpGet("initialized")]
  public async Task<ActionResult<IsConfigurationInitializedResult>> IsInitializedAsync(CancellationToken cancellationToken)
  {
    Configuration? configuration = await _configurationService.ReadAsync(cancellationToken);

    return Ok(new IsConfigurationInitializedResult(configuration));
  }

  [Authorize(Policy = Policies.PortalActor)]
  [HttpGet]
  public async Task<ActionResult<Configuration>> ReadAsync(CancellationToken cancellationToken)
  {
    Configuration configuration = await _configurationService.ReadAsync(cancellationToken)
      ?? throw new InvalidOperationException("The configuration has not been initialized.");

    return Ok(configuration);
  }

  [Authorize(Policy = Policies.PortalActor)]
  [HttpPut]
  public async Task<ActionResult<Configuration>> ReplaceAsync([FromBody] ReplaceConfigurationPayload payload, long? version, CancellationToken cancellationToken)
  {
    return Ok(await _configurationService.ReplaceAsync(payload, version, cancellationToken));
  }

  [Authorize(Policy = Policies.PortalActor)]
  [HttpPatch]
  public async Task<ActionResult<Configuration>> UpdateAsync([FromBody] UpdateConfigurationPayload payload, CancellationToken cancellationToken)
  {
    return Ok(await _configurationService.UpdateAsync(payload, cancellationToken));
  }
}
