using Logitar.Portal.Application.Realms;
using Logitar.Portal.Contracts.Realms;
using Logitar.Portal.Contracts.Search;
using Logitar.Portal.Web.Extensions;
using Logitar.Portal.Web.Models.Realms;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Logitar.Portal.Web.Controllers;

[ApiController]
[Authorize]
[Route("api/realms")]
public class RealmController : ControllerBase
{
  private readonly IRealmService _realmService;

  public RealmController(IRealmService realmService)
  {
    _realmService = realmService;
  }

  [HttpPost]
  public async Task<ActionResult<Realm>> CreateAsync([FromBody] CreateRealmPayload payload, CancellationToken cancellationToken)
  {
    Realm realm = await _realmService.CreateAsync(payload, cancellationToken);
    return Created(BuildLocation(realm), realm);
  }

  [HttpDelete("{id}")]
  public async Task<ActionResult<Realm>> DeleteAsync(Guid id, CancellationToken cancellationToken)
  {
    Realm? realm = await _realmService.DeleteAsync(id, cancellationToken);
    return realm == null ? NotFound() : Ok(realm);
  }

  [HttpGet("{id}")]
  public async Task<ActionResult<Realm>> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    Realm? realm = await _realmService.ReadAsync(id: id, cancellationToken: cancellationToken);
    return realm == null ? NotFound() : Ok(realm);
  }

  [HttpGet("unique-slug:{uniqueSlug}")]
  public async Task<ActionResult<Realm>> ReadByUniqueNameAsync(string uniqueSlug, CancellationToken cancellationToken)
  {
    Realm? realm = await _realmService.ReadAsync(uniqueSlug: uniqueSlug, cancellationToken: cancellationToken);
    return realm == null ? NotFound() : Ok(realm);
  }

  [HttpPut("{id}")]
  public async Task<ActionResult<Realm>> ReplaceAsync(Guid id, [FromBody] ReplaceRealmPayload payload, long? version, CancellationToken cancellationToken)
  {
    Realm? realm = await _realmService.ReplaceAsync(id, payload, version, cancellationToken);
    return realm == null ? NotFound() : Ok(realm);
  }

  [HttpGet]
  public async Task<ActionResult<SearchResults<Realm>>> SearchAsync([FromQuery] SearchRealmsModel model, CancellationToken cancellationToken)
  {
    SearchRealmsPayload payload = model.ToPayload();
    return Ok(await _realmService.SearchAsync(payload, cancellationToken));
  }

  [HttpPatch("{id}")]
  public async Task<ActionResult<Realm>> UpdateAsync(Guid id, [FromBody] UpdateRealmPayload payload, CancellationToken cancellationToken)
  {
    Realm? realm = await _realmService.UpdateAsync(id, payload, cancellationToken);
    return realm == null ? NotFound() : Ok(realm);
  }

  private Uri BuildLocation(Realm realm) => HttpContext.BuildLocation("realms/{id}", new Dictionary<string, string> { ["id"] = realm.Id.ToString() });
}
