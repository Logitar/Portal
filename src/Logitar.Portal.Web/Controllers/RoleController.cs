using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Constants;
using Logitar.Portal.Contracts.Roles;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Logitar.Portal.Web.Controllers;

[ApiController]
[Authorize(Policy = Policies.PortalActor)]
[Route("api/roles")]
public class RoleController : ControllerBase
{
  private readonly IRoleService _roleService;

  public RoleController(IRoleService roleService)
  {
    _roleService = roleService;
  }

  [HttpPost]
  public async Task<ActionResult<Role>> CreateAsync([FromBody] CreateRolePayload payload, CancellationToken cancellationToken)
  {
    Role role = await _roleService.CreateAsync(payload, cancellationToken);
    Uri uri = new($"{Request.Scheme}://{Request.Host}/api/roles/{role.Id}");

    return Created(uri, role);
  }

  [HttpDelete("{id}")]
  public async Task<ActionResult<Role>> DeleteAsync(Guid id, CancellationToken cancellationToken)
  {
    Role? role = await _roleService.DeleteAsync(id, cancellationToken);
    return role == null ? NotFound() : Ok(role);
  }

  [HttpGet("{id}")]
  public async Task<ActionResult<Role>> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    Role? role = await _roleService.ReadAsync(id, cancellationToken: cancellationToken);
    return role == null ? NotFound() : Ok(role);
  }

  [HttpGet("unique-name:{uniqueName}")]
  public async Task<ActionResult<Role>> ReadAsync(string uniqueName, string? realm, CancellationToken cancellationToken)
  {
    Role? role = await _roleService.ReadAsync(realm: realm, uniqueName: uniqueName, cancellationToken: cancellationToken);
    return role == null ? NotFound() : Ok(role);
  }

  [HttpPut("{id}")]
  public async Task<ActionResult<Role>> ReplaceAsync(Guid id, [FromBody] ReplaceRolePayload payload, long? version, CancellationToken cancellationToken)
  {
    Role? role = await _roleService.ReplaceAsync(id, payload, version, cancellationToken);
    return role == null ? NotFound() : Ok(role);
  }

  [HttpPost("search")]
  public async Task<ActionResult<SearchResults<Role>>> SearchAsync([FromBody] SearchRolesPayload payload, CancellationToken cancellationToken)
  {
    return Ok(await _roleService.SearchAsync(payload, cancellationToken));
  }

  [HttpPatch("{id}")]
  public async Task<ActionResult<Role>> UpdateAsync(Guid id, [FromBody] UpdateRolePayload payload, CancellationToken cancellationToken)
  {
    Role? role = await _roleService.UpdateAsync(id, payload, cancellationToken);
    return role == null ? NotFound() : Ok(role);
  }
}
