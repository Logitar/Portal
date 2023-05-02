using Logitar.Portal.Contracts.Sessions;
using Logitar.Portal.Core.Configurations;
using Logitar.Portal.Web.Commands;
using Logitar.Portal.Web.Constants;
using Logitar.Portal.Web.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Logitar.Portal.Web.Controllers.Api;

[ApiController]
[Route("api/configurations")]
public class ConfigurationApiController : ControllerBase
{
  private readonly IConfigurationService _configurationService;
  private readonly IMediator _mediator;

  public ConfigurationApiController(IConfigurationService configurationService, IMediator mediator)
  {
    _configurationService = configurationService;
    _mediator = mediator;
  }

  [Authorize(Policy = Policies.AuthenticatedPortalUser)]
  [HttpGet]
  public async Task<ActionResult<Configuration>> GetAsync(CancellationToken cancellationToken)
  {
    return Ok(await _configurationService.GetAsync(cancellationToken));
  }

  [HttpPost]
  public async Task<ActionResult<Configuration>> InitializeAsync([FromBody] InitializeConfigurationInput input, CancellationToken cancellationToken)
  {
    Configuration configuration = await _configurationService.InitializeAsync(input, cancellationToken);

    Session session = await _mediator.Send(new PortalSignIn(input), cancellationToken);
    HttpContext.SignIn(session);

    return Ok(configuration);
  }

  [Authorize(Policy = Policies.AuthenticatedPortalUser)]
  [HttpPut]
  public async Task<ActionResult<Configuration>> UpdateAsync([FromBody] UpdateConfigurationInput input, CancellationToken cancellationToken)
  {
    return Ok(await _configurationService.UpdateAsync(input, cancellationToken));
  }
}
