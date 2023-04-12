using Logitar.Portal.v2.Contracts.Realms;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Logitar.Portal.v2.Web.Controllers;

[ApiExplorerSettings(IgnoreApi = true)]
[Authorize(Policy = Constants.Policies.PortalActor)]
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

  [HttpGet("{idOrUniqueName}")]
  public async Task<ActionResult> RealmEdit(string idOrUniqueName, CancellationToken cancellationToken = default)
  {
    Guid? id = null;
    if (Guid.TryParse(idOrUniqueName, out Guid realmId))
    {
      id = realmId;
    }

    Realm? realm = await _realmService.GetAsync(id, idOrUniqueName, cancellationToken);
    if (realm == null)
    {
      return NotFound();
    }

    return View(realm);
  }

  [HttpGet]
  public ActionResult RealmList()
  {
    return View();
  }
}
