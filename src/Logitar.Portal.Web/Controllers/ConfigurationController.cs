using Logitar.Portal.Core.Configurations;
using Logitar.Portal.Web.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Logitar.Portal.Web.Controllers;

[ApiExplorerSettings(IgnoreApi = true)]
[Authorize(Policy = Policies.PortalActor)]
[Route("configuration")]
public class ConfigurationController : Controller
{
  private readonly IConfigurationService _configurationService;

  public ConfigurationController(IConfigurationService configurationService)
  {
    _configurationService = configurationService;
  }

  [HttpGet]
  public async Task<ActionResult> ConfigurationEdit(CancellationToken cancellationToken = default)
  {
    Configuration configuration = await _configurationService.GetAsync(cancellationToken)
      ?? throw new InvalidOperationException("The configuration should be initialized.");

    return View(configuration);
  }
}
