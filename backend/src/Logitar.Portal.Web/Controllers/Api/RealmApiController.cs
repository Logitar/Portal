using Logitar.Portal.Application.Realms;
using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Realms;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Logitar.Portal.Web.Controllers.Api
{
  [ApiController]
  [Authorize(Policy = Constants.Policies.PortalIdentity)]
  [Route("api/realms")]
  public class RealmApiController : ControllerBase
  {
    private readonly IRealmService _realmService;

    public RealmApiController(IRealmService realmService)
    {
      _realmService = realmService;
    }

    [HttpPost]
    public async Task<ActionResult<RealmModel>> CreateAsync([FromBody] CreateRealmPayload payload, CancellationToken cancellationToken)
    {
      RealmModel realm = await _realmService.CreateAsync(payload, cancellationToken);
      Uri uri = new($"/api/realms/{realm.Id}", UriKind.Relative);

      return Created(uri, realm);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteAsync(string id, CancellationToken cancellationToken)
    {
      await _realmService.DeleteAsync(id, cancellationToken);

      return NoContent();
    }

    [HttpGet]
    public async Task<ActionResult<ListModel<RealmModel>>> GetAsync(string? search,
      RealmSort? sort, bool desc,
      int? index, int? count,
      CancellationToken cancellationToken = default)
    {
      return Ok(await _realmService.GetAsync(search,
        sort, desc,
        index, count,
        cancellationToken));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<RealmModel>> GetAsync(string id, CancellationToken cancellationToken)
    {
      RealmModel? realm = await _realmService.GetAsync(id, cancellationToken);
      if (realm == null)
      {
        return NotFound();
      }

      return Ok(realm);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<RealmModel>> UpdateAsync(string id, [FromBody] UpdateRealmPayload payload, CancellationToken cancellationToken)
    {
      return Ok(await _realmService.UpdateAsync(id, payload, cancellationToken));
    }
  }
}
