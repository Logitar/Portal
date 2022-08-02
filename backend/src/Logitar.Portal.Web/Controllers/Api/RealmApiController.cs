using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Logitar.Portal.Core;
using Logitar.Portal.Core.Realms;
using Logitar.Portal.Core.Realms.Models;
using Logitar.Portal.Core.Realms.Payloads;

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
      var uri = new Uri($"/api/realms/{realm.Id}", UriKind.Relative);

      return Created(uri, realm);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<RealmModel>> DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
      return Ok(await _realmService.DeleteAsync(id, cancellationToken));
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
    public async Task<ActionResult<RealmModel>> UpdateAsync(Guid id, [FromBody] UpdateRealmPayload payload, CancellationToken cancellationToken)
    {
      return Ok(await _realmService.UpdateAsync(id, payload, cancellationToken));
    }
  }
}
