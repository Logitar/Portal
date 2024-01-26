using Logitar.Portal.Contracts.Roles;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Logitar.Portal.Controllers;

[ApiController]
[Authorize]
[Route("roles")]
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
    Uri uri = new($"{Request.Scheme}://{Request.Host}/roles/{role.Id}"); // TODO(fpion): refactor

    return Created(uri, role);
  }

  [HttpDelete("{id}")]
  public async Task<ActionResult<Role>> DeleteAsync(string id, CancellationToken cancellationToken)
  {
    Role? role = await _roleService.DeleteAsync(id, cancellationToken);
    return role == null ? NotFound() : Ok(role);
  }

  [HttpGet("{id}")]
  public async Task<ActionResult<Role>> ReadAsync(string id, CancellationToken cancellationToken)
  {
    Role? role = await _roleService.ReadAsync(id: id, cancellationToken: cancellationToken);
    return role == null ? NotFound() : Ok(role);
  }

  [HttpGet("unique-name:{uniqueName}")]
  public async Task<ActionResult<Role>> ReadByUniqueNameAsync(string uniqueName, CancellationToken cancellationToken)
  {
    Role? role = await _roleService.ReadAsync(uniqueName: uniqueName, cancellationToken: cancellationToken);
    return role == null ? NotFound() : Ok(role);
  }

  [HttpPut("{id}")]
  public async Task<ActionResult<Role>> ReplaceAsync(string id, [FromBody] ReplaceRolePayload payload, long? version, CancellationToken cancellationToken)
  {
    Role? role = await _roleService.ReplaceAsync(id, payload, version, cancellationToken);
    return role == null ? NotFound() : Ok(role);
  }

  [HttpPatch("{id}")]
  public async Task<ActionResult<Role>> UpdateAsync(string id, [FromBody] UpdateRolePayload payload, CancellationToken cancellationToken)
  {
    Role? role = await _roleService.UpdateAsync(id, payload, cancellationToken);
    return role == null ? NotFound() : Ok(role);
  }
}
