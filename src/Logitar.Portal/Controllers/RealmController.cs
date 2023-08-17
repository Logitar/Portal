using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Realms;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Logitar.Portal.Controllers;

[ApiController]
[Authorize] // TODO(fpion): PortalIdentity
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
    Uri uri = new($"{Request.Scheme}://{Request.Host}/realms/{realm.Id}");

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
    Realm? realm = await _realmService.ReadAsync(id, cancellationToken: cancellationToken);

    return realm == null ? NotFound() : Ok(realm);
  }

  [HttpGet]
  public async Task<ActionResult<Realm>> ReadAsync(string? id, string? uniqueSlug, CancellationToken cancellationToken)
  {
    Realm? realm = await _realmService.ReadAsync(id, uniqueSlug, cancellationToken);

    return realm == null ? NotFound() : Ok(realm);
  }

  [HttpPut("{id}")]
  public async Task<ActionResult<Realm>> ReplaceAsync(string id, [FromBody] ReplaceRealmPayload payload, long? version, CancellationToken cancellationToken)
  {
    Realm? realm = await _realmService.ReplaceAsync(id, payload, version, cancellationToken);

    return realm == null ? NotFound() : Ok(realm);
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
