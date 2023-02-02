using Logitar.Portal.Application.Realms;
using Logitar.Portal.Contracts.Realms;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Logitar.Portal.Web.Controllers
{
  [ApiExplorerSettings(IgnoreApi = true)]
  [Authorize(Policy = Constants.Policies.PortalIdentity)]
  [Route("realms")]
  public class RealmController : Controller
  {
    private readonly IRealmService _realmService;

    public RealmController(IRealmService realmService)
    {
      _realmService = realmService;
    }

    [HttpGet("/create-realm")]
    public ActionResult CreateRealm()
    {
      return View(nameof(RealmEdit));
    }

    [HttpGet]
    public ActionResult RealmList()
    {
      return View();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult> RealmEdit(string id, CancellationToken cancellationToken = default)
    {
      RealmModel? realm = await _realmService.GetAsync(id, cancellationToken);
      if (realm == null)
      {
        return NotFound();
      }

      return View(realm);
    }
  }
}
