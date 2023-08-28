using Microsoft.AspNetCore.Mvc;

namespace Logitar.Portal.Web.Controllers;

[ApiExplorerSettings(IgnoreApi = true)]
[Route("app")]
public class AppController : Controller
{
  [HttpGet("{**anything}")]
  public ActionResult Index()
  {
    return View();
  }
}
