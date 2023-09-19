using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Constants;
using Logitar.Portal.Contracts.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Logitar.Portal.Web.Controllers;

[ApiController]
[Authorize(Policy = Policies.PortalActor)]
[Route("api/users")]
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
    Uri uri = new($"{Request.Scheme}://{Request.Host}/api/users/{user.Id}");

    return Created(uri, user);
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
    User? user = await _userService.ReadAsync(id, cancellationToken: cancellationToken);
    return user == null ? NotFound() : Ok(user);
  }

  [HttpGet("unique-name:{uniqueName}")]
  public async Task<ActionResult<User>> ReadAsync(string uniqueName, string? realm, CancellationToken cancellationToken)
  {
    User? user = await _userService.ReadAsync(realm: realm, uniqueName: uniqueName, cancellationToken: cancellationToken);
    return user == null ? NotFound() : Ok(user);
  }

  [HttpGet("identifiers/key:{key}/value:{value}")]
  public async Task<ActionResult<User>> ReadByIdentifierAsync(string key, string value, string? realm, CancellationToken cancellationToken)
  {
    User? user = await _userService.ReadAsync(realm: realm, identifierKey: key, identifierValue: value, cancellationToken: cancellationToken);
    return user == null ? NotFound() : Ok(user);
  }

  [HttpPost("password/recover")]
  public async Task<ActionResult<RecoverPasswordResult>> RecoverPasswordAsync([FromBody] RecoverPasswordPayload payload, CancellationToken cancellationToken)
  {
    RecoverPasswordResult result = await _userService.RecoverPasswordAsync(payload, cancellationToken);
    Uri uri = new($"{Request.Scheme}://{Request.Host}/api/messages/{result.MessageId}");

    return Created(uri, result);
  }

  [HttpDelete("{id}/identifiers/key:{key}")]
  public async Task<ActionResult<User>> RemoveIdentifierAsync(Guid id, string key, CancellationToken cancellationToken)
  {
    User? user = await _userService.RemoveIdentifierAsync(id, key, cancellationToken);
    return user == null ? NotFound() : Ok(user);
  }

  [HttpPost("password/reset")]
  public async Task<ActionResult<User>> ResetPasswordAsync([FromBody] ResetPasswordPayload payload, CancellationToken cancellationToken)
  {
    return Ok(await _userService.ResetPasswordAsync(payload, cancellationToken));
  }

  [HttpPut("{id}")]
  public async Task<ActionResult<User>> ReplaceAsync(Guid id, [FromBody] ReplaceUserPayload payload, long? version, CancellationToken cancellationToken)
  {
    User? user = await _userService.ReplaceAsync(id, payload, version, cancellationToken);
    return user == null ? NotFound() : Ok(user);
  }

  [HttpPut("{id}/identifiers")]
  public async Task<ActionResult<User>> SaveIdentifierAsync(Guid id, [FromBody] SaveIdentifierPayload payload, CancellationToken cancellationToken)
  {
    User? user = await _userService.SaveIdentifierAsync(id, payload, cancellationToken);
    return user == null ? NotFound() : Ok(user);
  }

  [HttpPost("search")]
  public async Task<ActionResult<SearchResults<User>>> SearchAsync([FromBody] SearchUsersPayload payload, CancellationToken cancellationToken)
  {
    return Ok(await _userService.SearchAsync(payload, cancellationToken));
  }

  [HttpPatch("{id}/sign/out")]
  public async Task<ActionResult<User>> SignOutAsync(Guid id, CancellationToken cancellationToken)
  {
    User? user = await _userService.SignOutAsync(id, cancellationToken);
    return user == null ? NotFound() : Ok(user);
  }

  [HttpPatch("{id}")]
  public async Task<ActionResult<User>> UpdateAsync(Guid id, [FromBody] UpdateUserPayload payload, CancellationToken cancellationToken)
  {
    User? user = await _userService.UpdateAsync(id, payload, cancellationToken);
    return user == null ? NotFound() : Ok(user);
  }
}
