using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Logitar.Portal.Web.Controllers
{
  [ApiExplorerSettings(IgnoreApi = true)]
  [Authorize(Policy = Constants.Policies.PortalIdentity)]
  [Route("tokens")]
  public class TokenController : Controller
  {
    [HttpGet]
    public ActionResult Token() => View();
  }
}
