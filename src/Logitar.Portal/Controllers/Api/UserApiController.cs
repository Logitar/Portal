using Logitar.Portal.Constants;
using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Logitar.Portal.Controllers.Api;

[ApiController]
[Authorize(Policies.PortalActor)]
[Route("api/users")]
public class UserApiController : ControllerBase
{
  private readonly IUserService _userService;

  public UserApiController(IUserService userService)
  {
    _userService = userService;
  }

  [HttpPost]
  public async Task<ActionResult<User>> CreateAsync([FromBody] CreateUserPayload payload, CancellationToken cancellationToken)
  {
    User user = await _userService.CreateAsync(payload, cancellationToken);
    Uri uri = new($"{Request.Scheme}://{Request.Host}/api/users/{user.Id}");

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
    User? user = await _userService.ReadAsync(id: id, cancellationToken: cancellationToken);
    return user == null ? NotFound() : Ok(user);
  }

  [HttpGet("unique-name:{uniqueName}")]
  public async Task<ActionResult<User>> ReadByUniqueSlugAsync(string? realm, string uniqueName, CancellationToken cancellationToken)
  {
    User? user = await _userService.ReadAsync(realm: realm, uniqueName: uniqueName, cancellationToken: cancellationToken);
    return user == null ? NotFound() : Ok(user);
  }

  [HttpPut("{id}")]
  public Task<ActionResult<User>> ReplaceAsync(string id, long? version, /*[FromBody] ReplaceUserPayload payload,*/ CancellationToken cancellationToken)
  {
    throw new NotImplementedException();
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
