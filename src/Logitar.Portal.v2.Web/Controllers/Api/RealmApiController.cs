using Logitar.Portal.v2.Contracts;
using Logitar.Portal.v2.Contracts.Realms;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Logitar.Portal.v2.Web.Controllers.Api;

[ApiController]
[Authorize(Policy = Constants.Policies.PortalActor)]
[Route("api/realms")]
public class RealmApiController : ControllerBase
{
  private readonly IRealmService _realmService;

  public RealmApiController(IRealmService realmService)
  {
    _realmService = realmService;
  }

  [HttpPost]
  public async Task<ActionResult<Realm>> CreateAsync([FromBody] CreateRealmInput input, CancellationToken cancellationToken)
  {
    Realm realm = await _realmService.CreateAsync(input, cancellationToken);
    Uri uri = new($"{Request.Scheme}://{Request.Host}/api/realms/{realm.Id}");

    return Created(uri, realm);
  }

  [HttpDelete("{id}")]
  public async Task<ActionResult<Realm>> DeleteAsync(Guid id, CancellationToken cancellationToken)
  {
    return Ok(await _realmService.DeleteAsync(id, cancellationToken));
  }

  [HttpGet]
  public async Task<ActionResult<PagedList<Realm>>> GetAsync(string? search, RealmSort? sort,
    bool isDescending, int? skip, int? limit, CancellationToken cancellationToken)
  {
    return Ok(await _realmService.GetAsync(search, sort, isDescending, skip, limit, cancellationToken));
  }

  [HttpGet("{idOrUniqueName}")]
  public async Task<ActionResult<Realm>> GetAsync(string idOrUniqueName, CancellationToken cancellationToken)
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

    return Ok(realm);
  }

  [HttpPut("{id}")]
  public async Task<ActionResult<Realm>> UpdateAsync(Guid id, [FromBody] UpdateRealmInput input, CancellationToken cancellationToken)
  {
    return Ok(await _realmService.UpdateAsync(id, input, cancellationToken));
  }
}
