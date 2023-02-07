using Logitar.Portal.Application.Configurations;
using Logitar.Portal.Application.Configurations.Payloads;
using Microsoft.AspNetCore.Mvc;

namespace Logitar.Portal.Web.Controllers.Api
{
  [ApiController]
  [Route("api/configurations")]
  public class ConfigurationApiController : ControllerBase
  {
    private readonly IConfigurationService _configurationService;

    public ConfigurationApiController(IConfigurationService configurationService)
    {
      _configurationService = configurationService;
    }

    [HttpPost]
    public async Task<ActionResult> InitializeAsync([FromBody] InitializeConfigurationPayload payload, CancellationToken cancellationToken)
    {
      await _configurationService.InitializeAsync(payload, cancellationToken);

      return NoContent();
    }
  }
}
