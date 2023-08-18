using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Logitar.Portal.Controllers;

[ApiController]
[Authorize] // TODO(fpion): PortalIdentity
[Route("users")]
public class UserController : ControllerBase
{
  private readonly IUserService _userService;

  public UserController(IUserService userService)
  {
    _userService = userService;
  }

  [HttpPost]
  public async Task<ActionResult<User>> CreateAsync([FromBody] CreateUserPayload payload, CancellationToken cancellationToken)
  {
    User user = await _userService.CreateAsync(payload, cancellationToken);
    Uri uri = new($"{Request.Scheme}://{Request.Host}/users/{user.Id}");

    return Created(uri, user);
  }

  [HttpDelete("{id}")]
  public async Task<ActionResult<User>> DeleteAsync(string id, CancellationToken cancellationToken)
  {
    User? user = await _userService.DeleteAsync(id, cancellationToken);

    return user == null ? NotFound() : Ok(user);
  }

  [HttpGet("{id}")]
  public async Task<ActionResult<User>> ReadAsync(string id, CancellationToken cancellationToken)
  {
    User? user = await _userService.ReadAsync(id, cancellationToken: cancellationToken);

    return user == null ? NotFound() : Ok(user);
  }

  [HttpGet]
  public async Task<ActionResult<User>> ReadAsync(string? id, string? realm, string? uniqueName, CancellationToken cancellationToken)
  {
    User? user = await _userService.ReadAsync(id, realm, uniqueName, cancellationToken);

    return user == null ? NotFound() : Ok(user);
  }

  [HttpPut("{id}")]
  public async Task<ActionResult<User>> ReplaceAsync(string id, [FromBody] ReplaceUserPayload payload, long? version, CancellationToken cancellationToken)
  {
    User? user = await _userService.ReplaceAsync(id, payload, version, cancellationToken);

    return user == null ? NotFound() : Ok(user);
  }

  [HttpPost("search")]
  public async Task<ActionResult<SearchResults<User>>> SearchAsync([FromBody] SearchUsersPayload payload, CancellationToken cancellationToken)
  {
    return Ok(await _userService.SearchAsync(payload, cancellationToken));
  }

  [HttpPatch("{id}")]
  public async Task<ActionResult<User>> UpdateAsync(string id, [FromBody] UpdateUserPayload payload, CancellationToken cancellationToken)
  {
    User? user = await _userService.UpdateAsync(id, payload, cancellationToken);

    return user == null ? NotFound() : Ok(user);
  }
}
