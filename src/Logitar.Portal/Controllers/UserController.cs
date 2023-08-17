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
}
