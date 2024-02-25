using Logitar.Portal.Contracts.Configurations;
using Logitar.Portal.Contracts.Sessions;
using Logitar.Portal.Web.Extensions;
using Logitar.Portal.Web.Models.Account;
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

  [HttpPost("initialize")]
  public async Task<ActionResult<CurrentUser>> InitializeAsync([FromBody] InitializeConfigurationModel model, CancellationToken cancellationToken)
  {
    InitializeConfigurationPayload payload = model.ToPayload(HttpContext.GetSessionCustomAttributes());
    Session session = await _configurationService.InitializeAsync(payload, cancellationToken);
    HttpContext.SignIn(session);

    return Ok(new CurrentUser(session));
  }

  [HttpGet("initialized")]
  public async Task<ActionResult<IsConfigurationInitialized>> IsInitializedAsync(CancellationToken cancellationToken)
  {
    Configuration? configuration = await _configurationService.ReadAsync(cancellationToken);
    return Ok(new IsConfigurationInitialized(configuration));
  }

  [Authorize]
  [HttpGet]
  public async Task<ActionResult<IsConfigurationInitialized>> ReadAsync(CancellationToken cancellationToken)
  {
    Configuration configuration = await _configurationService.ReadAsync(cancellationToken)
      ?? throw new InvalidOperationException("The configuration cannot be null if an actor is authenticated.");
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
