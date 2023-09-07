using Logitar.Portal.Contracts.Configurations;
using Logitar.Portal.Contracts.Constants;
using Logitar.Portal.Contracts.Users;
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
  public async Task<ActionResult<InitializeConfigurationResult>> InitializeAsync([FromBody] InitializeConfigurationPayload payload, CancellationToken cancellationToken)
  {
    payload.Session = new SessionPayload
    {
      CustomAttributes = HttpContext.GetSessionCustomAttributes()
    };
    InitializeConfigurationResult result = await _configurationService.InitializeAsync(payload, cancellationToken);

    HttpContext.SignIn(result.Session);

    User user = result.Session.User;
    Uri uri = new($"{Request.Scheme}://{Request.Host}/api/users/{user.Id}");

    return Created(uri, result);
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
