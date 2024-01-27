using Logitar.Portal.Contracts.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Logitar.Portal.Controllers;

[ApiController]
[Authorize]
[Route("users")]
public class UserController : ControllerBase
{
  private readonly IUserService _userService;

  public UserController(IUserService userService)
  {
    _userService = userService;
  }

  [HttpPost("authenticate")]
  public async Task<ActionResult<User>> AuthenticateAsync([FromBody] AuthenticateUserPayload payload, CancellationToken cancellationToken)
  {
    return Ok(await _userService.AuthenticateAsync(payload, cancellationToken));
  }

  [HttpPost]
  public async Task<ActionResult<User>> CreateAsync([FromBody] CreateUserPayload payload, CancellationToken cancellationToken)
  {
    User user = await _userService.CreateAsync(payload, cancellationToken);
    Uri uri = new($"{Request.Scheme}://{Request.Host}/users/{user.Id}"); // TODO(fpion): refactor

    return Created(uri, user);
  }

  [HttpDelete("{id}")]
  public async Task<ActionResult<User>> DeleteAsync(string id, CancellationToken cancellationToken)
  {
    User? user = await _userService.DeleteAsync(id, cancellationToken);
    return user == null ? NotFound() : Ok(user);
  }

  [HttpPut("{id}")]
  public async Task<ActionResult<User>> ReplaceAsync(string id, [FromBody] ReplaceUserPayload payload, long? version, CancellationToken cancellationToken)
  {
    User? user = await _userService.ReplaceAsync(id, payload, version, cancellationToken);
    return user == null ? NotFound() : Ok(user);
  }

  [HttpPatch("{id}/password/reset")]
  public async Task<ActionResult<User>> ResetPasswordAsync(string id, [FromBody] ResetUserPasswordPayload payload, CancellationToken cancellationToken)
  {
    User? user = await _userService.ResetPasswordAsync(id, payload, cancellationToken);
    return user == null ? NotFound() : Ok(user);
  }

  [HttpPost("{id}/sign/out")]
  public async Task<ActionResult<User>> SignOutAsync(string id, CancellationToken cancellationToken)
  {
    User? user = await _userService.SignOutAsync(id, cancellationToken);
    return user == null ? NotFound() : Ok(user);
  }

  [HttpPatch("{id}")]
  public async Task<ActionResult<User>> UpdateAsync(string id, [FromBody] UpdateUserPayload payload, CancellationToken cancellationToken)
  {
    User? user = await _userService.UpdateAsync(id, payload, cancellationToken);
    return user == null ? NotFound() : Ok(user);
  }
}
