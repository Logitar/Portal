﻿using Logitar.Portal.v2.Core.Configurations;
using Microsoft.AspNetCore.Mvc;

namespace Logitar.Portal.v2.Web.Controllers;

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
