using Logitar.Portal.Application.Roles;
using Logitar.Portal.Contracts.Roles;
using Logitar.Portal.Contracts.Search;
using Logitar.Portal.Web.Extensions;
using Logitar.Portal.Web.Models.Roles;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Logitar.Portal.Web.Controllers;

[ApiController]
[Authorize]
[Route("api/roles")]
public class RoleController : ControllerBase
{
  private readonly IRoleService _roleService;

  public RoleController(IRoleService roleService)
  {
    _roleService = roleService;
  }

  [HttpPost]
  public async Task<ActionResult<RoleModel>> CreateAsync([FromBody] CreateRolePayload payload, CancellationToken cancellationToken)
  {
    RoleModel role = await _roleService.CreateAsync(payload, cancellationToken);
    return Created(BuildLocation(role), role);
  }

  [HttpDelete("{id}")]
  public async Task<ActionResult<RoleModel>> DeleteAsync(Guid id, CancellationToken cancellationToken)
  {
    RoleModel? role = await _roleService.DeleteAsync(id, cancellationToken);
    return role == null ? NotFound() : Ok(role);
  }

  [HttpGet("{id}")]
  public async Task<ActionResult<RoleModel>> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    RoleModel? role = await _roleService.ReadAsync(id: id, cancellationToken: cancellationToken);
    return role == null ? NotFound() : Ok(role);
  }

  [HttpGet("unique-name:{uniqueName}")]
  public async Task<ActionResult<RoleModel>> ReadByUniqueNameAsync(string uniqueName, CancellationToken cancellationToken)
  {
    RoleModel? role = await _roleService.ReadAsync(uniqueName: uniqueName, cancellationToken: cancellationToken);
    return role == null ? NotFound() : Ok(role);
  }

  [HttpPut("{id}")]
  public async Task<ActionResult<RoleModel>> ReplaceAsync(Guid id, [FromBody] ReplaceRolePayload payload, long? version, CancellationToken cancellationToken)
  {
    RoleModel? role = await _roleService.ReplaceAsync(id, payload, version, cancellationToken);
    return role == null ? NotFound() : Ok(role);
  }

  [HttpGet]
  public async Task<ActionResult<SearchResults<RoleModel>>> SearchAsync([FromQuery] SearchRolesModel model, CancellationToken cancellationToken)
  {
    SearchRolesPayload payload = model.ToPayload();
    return Ok(await _roleService.SearchAsync(payload, cancellationToken));
  }

  [HttpPatch("{id}")]
  public async Task<ActionResult<RoleModel>> UpdateAsync(Guid id, [FromBody] UpdateRolePayload payload, CancellationToken cancellationToken)
  {
    RoleModel? role = await _roleService.UpdateAsync(id, payload, cancellationToken);
    return role == null ? NotFound() : Ok(role);
  }

  private Uri BuildLocation(RoleModel role) => HttpContext.BuildLocation("roles/{id}", new Dictionary<string, string> { ["id"] = role.Id.ToString() });
}
