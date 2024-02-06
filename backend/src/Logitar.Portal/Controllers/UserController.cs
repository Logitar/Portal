using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Search;
using Logitar.Portal.Contracts.Users;
using Logitar.Portal.Extensions;
using Logitar.Portal.Models.Users;
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

  [HttpPatch("authenticate")]
  public async Task<ActionResult<User>> AuthenticateAsync([FromBody] AuthenticateUserPayload payload, CancellationToken cancellationToken)
  {
    return Ok(await _userService.AuthenticateAsync(payload, cancellationToken));
  }

  [HttpPost]
  public async Task<ActionResult<User>> CreateAsync([FromBody] CreateUserPayload payload, CancellationToken cancellationToken)
  {
    User user = await _userService.CreateAsync(payload, cancellationToken);
    return Created(BuildLocation(user), user);
  }

  [HttpDelete("{id}")]
  public async Task<ActionResult<User>> DeleteAsync(Guid id, CancellationToken cancellationToken)
  {
    User? user = await _userService.DeleteAsync(id, cancellationToken);
    return user == null ? NotFound() : Ok(user);
  }

  [HttpGet("{id}")]
  public async Task<ActionResult<User>> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    User? user = await _userService.ReadAsync(id: id, cancellationToken: cancellationToken);
    return user == null ? NotFound() : Ok(user);
  }

  [HttpGet("identifier/key:{key}/value:{value}")]
  public async Task<ActionResult<User>> ReadAsync(string key, string value, CancellationToken cancellationToken)
  {
    CustomIdentifier identifier = new(key, value);
    User? user = await _userService.ReadAsync(identifier: identifier, cancellationToken: cancellationToken);
    return user == null ? NotFound() : Ok(user);
  }

  [HttpGet("unique-name:{uniqueName}")]
  public async Task<ActionResult<User>> ReadAsync(string uniqueName, CancellationToken cancellationToken)
  {
    User? user = await _userService.ReadAsync(uniqueName: uniqueName, cancellationToken: cancellationToken);
    return user == null ? NotFound() : Ok(user);
  }

  [HttpDelete("{id}/identifiers/key:{key}")]
  public async Task<ActionResult<User>> RemoveIdentifierAsync(Guid id, string key, CancellationToken cancellationToken)
  {
    User? user = await _userService.RemoveIdentifierAsync(id, key, cancellationToken);
    return user == null ? NotFound() : Ok(user);
  }

  [HttpPatch("{id}/password/reset")]
  public async Task<ActionResult<User>> ResetPasswordAsync(Guid id, [FromBody] ResetUserPasswordPayload payload, CancellationToken cancellationToken)
  {
    User? user = await _userService.ResetPasswordAsync(id, payload, cancellationToken);
    return user == null ? NotFound() : Ok();
  }

  [HttpPut("{id}/identifiers/key:{key}")]
  public async Task<ActionResult<User>> SaveIdentifierAsync(Guid id, string key, [FromBody] SaveUserIdentifierPayload payload, CancellationToken cancellationToken)
  {
    User? user = await _userService.SaveIdentifierAsync(id, key, payload, cancellationToken);
    return user == null ? NotFound() : Ok(user);
  }

  [HttpGet]
  public async Task<ActionResult<SearchResults<User>>> SearchAsync([FromQuery] SearchUsersModel model, CancellationToken cancellationToken)
  {
    SearchUsersPayload payload = model.ToPayload();
    return Ok(await _userService.SearchAsync(payload, cancellationToken));
  }

  [HttpPut("{id}/sign/out")]
  public async Task<ActionResult<User>> SignOutAsync(Guid id, CancellationToken cancellationToken)
  {
    User? user = await _userService.SignOutAsync(id, cancellationToken);
    return user == null ? NotFound() : Ok(user);
  }

  private Uri BuildLocation(User user) => HttpContext.BuildLocation("users/{id}", new Dictionary<string, string> { ["id"] = user.Id.ToString() });
}
