using Logitar.Portal.Constants;
using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Realms;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Logitar.Portal.Controllers.Api;

[ApiController]
[Authorize(Policies.PortalActor)]
[Route("api/realms")]
public class RealmApiController : ControllerBase
{
  private readonly IRealmService _realmService;

  public RealmApiController(IRealmService realmService)
  {
    _realmService = realmService;
  }

  [HttpPost]
  public async Task<ActionResult<Realm>> CreateAsync([FromBody] CreateRealmPayload payload, CancellationToken cancellationToken)
  {
    Realm realm = await _realmService.CreateAsync(payload, cancellationToken);
    Uri uri = new($"{Request.Scheme}://{Request.Host}/api/realms/{realm.Id}");

    return Created(uri, realm);
  }

  [HttpDelete("{id}")]
  public async Task<ActionResult<Realm>> DeleteAsync(string id, CancellationToken cancellationToken)
  {
    Realm? realm = await _realmService.DeleteAsync(id, cancellationToken);
    return realm == null ? NotFound() : Ok(realm);
  }

  [HttpGet("{id}")]
  public async Task<ActionResult<Realm>> ReadAsync(string id, CancellationToken cancellationToken)
  {
    Realm? realm = await _realmService.ReadAsync(id: id, cancellationToken: cancellationToken);
    return realm == null ? NotFound() : Ok(realm);
  }

  [HttpGet("unique-slug:{uniqueSlug}")]
  public async Task<ActionResult<Realm>> ReadByUniqueSlugAsync(string uniqueSlug, CancellationToken cancellationToken)
  {
    Realm? realm = await _realmService.ReadAsync(uniqueSlug: uniqueSlug, cancellationToken: cancellationToken);
    return realm == null ? NotFound() : Ok(realm);
  }

  [HttpPut("{id}")]
  public Task<ActionResult<Realm>> ReplaceAsync(string id, long? version, /*[FromBody] ReplaceRealmPayload payload,*/ CancellationToken cancellationToken)
  {
    throw new NotImplementedException();
  }

  [HttpPost("search")]
  public async Task<ActionResult<SearchResults<Realm>>> SearchAsync([FromBody] SearchRealmsPayload payload, CancellationToken cancellationToken)
  {
    return Ok(await _realmService.SearchAsync(payload, cancellationToken));
  }

  [HttpPatch("{id}")]
  public async Task<ActionResult<Realm>> UpdateAsync(string id, [FromBody] UpdateRealmPayload payload, CancellationToken cancellationToken)
  {
    Realm? realm = await _realmService.UpdateAsync(id, payload, cancellationToken);
    return realm == null ? NotFound() : Ok(realm);
  }
}
