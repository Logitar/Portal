using Logitar.Portal.v2.Contracts.Sessions;
using Logitar.Portal.v2.Core.Configurations;
using Logitar.Portal.v2.Web.Commands;
using Logitar.Portal.v2.Web.Extensions;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Logitar.Portal.v2.Web.Controllers.Api;

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

  [HttpPost]
  public async Task<ActionResult> InitializeAsync([FromBody] InitializeConfigurationInput input, CancellationToken cancellationToken)
  {
    if (await _configurationService.IsInitializedAsync(cancellationToken))
    {
      return Forbid();
    }

    Uri url = new($"{Request.Scheme}://{Request.Host}");

    await _configurationService.InitializeAsync(input, url, cancellationToken);

    Session session = await _mediator.Send(new PortalSignIn(input), cancellationToken);
    HttpContext.SignIn(session);

    return NoContent();
  }
}
