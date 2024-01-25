using Logitar.Portal.Contracts.Configurations;
using Logitar.Portal.Contracts.Sessions;
using Logitar.Portal.Extensions;
using Logitar.Portal.Models.Account;
using Logitar.Portal.Models.Configuration;
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

  [HttpPost("initialize")]
  public async Task<ActionResult<CurrentUser>> InitializeAsync([FromBody] InitializeConfigurationPayload payload, CancellationToken cancellationToken)
  {
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
}
