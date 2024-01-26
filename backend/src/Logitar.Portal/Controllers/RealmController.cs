using Logitar.Portal.Contracts.Realms;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Logitar.Portal.Controllers;

[ApiController]
[Authorize]
[Route("realms")]
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
    Uri uri = new($"{Request.Scheme}://{Request.Host}/realms/{realm.Id}"); // TODO(fpion): refactor

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
  public async Task<ActionResult<Realm>> ReplaceAsync(string id, [FromBody] ReplaceRealmPayload payload, long? version, CancellationToken cancellationToken)
  {
    Realm? realm = await _realmService.ReplaceAsync(id, payload, version, cancellationToken);
    return realm == null ? NotFound() : Ok(realm);
  }

  [HttpPatch("{id}")]
  public async Task<ActionResult<Realm>> UpdateAsync(string id, [FromBody] UpdateRealmPayload payload, CancellationToken cancellationToken)
  {
    Realm? realm = await _realmService.UpdateAsync(id, payload, cancellationToken);
    return realm == null ? NotFound() : Ok(realm);
  }
}
