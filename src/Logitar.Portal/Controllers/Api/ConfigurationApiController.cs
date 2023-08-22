using Logitar.Portal.Constants;
using Logitar.Portal.Contracts.Configurations;
using Logitar.Portal.Contracts.Sessions;
using Logitar.Portal.Extensions;
using Logitar.Portal.Models.Api.Configuration;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Logitar.Portal.Controllers.Api;

[ApiController]
[Route("api/configuration")]
public class ConfigurationApiController : ControllerBase
{
  private readonly IConfigurationService _configurationService;
  private readonly ISessionService _sessionService;

  public ConfigurationApiController(IConfigurationService configurationService, ISessionService sessionService)
  {
    _configurationService = configurationService;
    _sessionService = sessionService;
  }

  [HttpPost]
  public async Task<ActionResult> InitializeAsync([FromBody] InitializeConfigurationPayload payload, CancellationToken cancellationToken)
  {
    InitializeConfigurationResult result = await _configurationService.InitializeAsync(payload, cancellationToken);

    Session session = await _sessionService.CreateAsync(new CreateSessionPayload
    {
      UserId = result.User.Id,
      IsPersistent = false,
      CustomAttributes = HttpContext.GetSessionCustomAttributes()
    }, cancellationToken);
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
  [HttpPatch]
  public async Task<ActionResult<Configuration>> UpdateAsync([FromBody] UpdateConfigurationPayload payload, CancellationToken cancellationToken)
  {
    return Ok(await _configurationService.UpdateAsync(payload, cancellationToken));
  }
}
