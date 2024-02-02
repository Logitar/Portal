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

  [HttpPost]
  public async Task<ActionResult<User>> CreateAsync([FromBody] CreateUserPayload payload, CancellationToken cancellationToken)
  {
    User user = await _userService.CreateAsync(payload, cancellationToken);
    return Created(GetLocation(user), user);
  }

  [HttpPost("{id}/sign/out")]
  public async Task<ActionResult<User>> SignOutAsync(Guid id, CancellationToken cancellationToken)
  {
    User? user = await _userService.SignOutAsync(id, cancellationToken);
    return user == null ? NotFound() : Ok(user);
  }

  private Uri GetLocation(User user) => new($"{Request.Scheme}://{Request.Host}/users/{user.Id}"); // TODO(fpion): refactor
}
