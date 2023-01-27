using Logitar.Portal.Core2.Configurations;
using Microsoft.AspNetCore.Mvc;

namespace Logitar.Portal.Web2.Controllers
{
  [ApiExplorerSettings(IgnoreApi = true)]
  [Route("")]
  public class HomeController : Controller
  {
    private readonly IConfigurationService _configurationService;

    public HomeController(IConfigurationService configurationService)
    {
      _configurationService = configurationService;
    }

    [HttpGet]
    public async Task<ActionResult> Index(CancellationToken cancellationToken)
    {
      if (await _configurationService.IsInitializedAsync(cancellationToken))
      {
        return RedirectToAction(actionName: "SignIn", controllerName: "Account");
      }

      return View();
    }
  }
}
