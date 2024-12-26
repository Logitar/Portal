using Logitar.Portal.Application.Users;
using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Search;
using Logitar.Portal.Contracts.Users;
using Logitar.Portal.Web.Extensions;
using Logitar.Portal.Web.Models.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Logitar.Portal.Web.Controllers;

[ApiController]
[Authorize]
[Route("api/users")]
public class UserController : ControllerBase
{
  private readonly IUserService _userService;

  public UserController(IUserService userService)
  {
    _userService = userService;
  }

  [HttpPatch("authenticate")]
  public async Task<ActionResult<UserModel>> AuthenticateAsync([FromBody] AuthenticateUserPayload payload, CancellationToken cancellationToken)
  {
    return Ok(await _userService.AuthenticateAsync(payload, cancellationToken));
  }

  [HttpPost]
  public async Task<ActionResult<UserModel>> CreateAsync([FromBody] CreateUserPayload payload, CancellationToken cancellationToken)
  {
    UserModel user = await _userService.CreateAsync(payload, cancellationToken);
    return Created(BuildLocation(user), user);
  }

  [HttpDelete("{id}")]
  public async Task<ActionResult<UserModel>> DeleteAsync(Guid id, CancellationToken cancellationToken)
  {
    UserModel? user = await _userService.DeleteAsync(id, cancellationToken);
    return user == null ? NotFound() : Ok(user);
  }

  [HttpGet("{id}")]
  public async Task<ActionResult<UserModel>> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    UserModel? user = await _userService.ReadAsync(id: id, cancellationToken: cancellationToken);
    return user == null ? NotFound() : Ok(user);
  }

  [HttpGet("identifier/key:{key}/value:{value}")]
  public async Task<ActionResult<UserModel>> ReadAsync(string key, string value, CancellationToken cancellationToken)
  {
    CustomIdentifier identifier = new(key, value);
    UserModel? user = await _userService.ReadAsync(identifier: identifier, cancellationToken: cancellationToken);
    return user == null ? NotFound() : Ok(user);
  }

  [HttpGet("unique-name:{uniqueName}")]
  public async Task<ActionResult<UserModel>> ReadAsync(string uniqueName, CancellationToken cancellationToken)
  {
    UserModel? user = await _userService.ReadAsync(uniqueName: uniqueName, cancellationToken: cancellationToken);
    return user == null ? NotFound() : Ok(user);
  }

  [HttpDelete("{id}/identifiers/key:{key}")]
  public async Task<ActionResult<UserModel>> RemoveIdentifierAsync(Guid id, string key, CancellationToken cancellationToken)
  {
    UserModel? user = await _userService.RemoveIdentifierAsync(id, key, cancellationToken);
    return user == null ? NotFound() : Ok(user);
  }

  [HttpPut("{id}")]
  public async Task<ActionResult<UserModel>> ReplaceAsync(Guid id, [FromBody] ReplaceUserPayload payload, long? version, CancellationToken cancellationToken)
  {
    UserModel? user = await _userService.ReplaceAsync(id, payload, version, cancellationToken);
    return user == null ? NotFound() : Ok(user);
  }

  [HttpPatch("{id}/password/reset")]
  public async Task<ActionResult<UserModel>> ResetPasswordAsync(Guid id, [FromBody] ResetUserPasswordPayload payload, CancellationToken cancellationToken)
  {
    UserModel? user = await _userService.ResetPasswordAsync(id, payload, cancellationToken);
    return user == null ? NotFound() : Ok(user);
  }

  [HttpPut("{id}/identifiers/key:{key}")]
  public async Task<ActionResult<UserModel>> SaveIdentifierAsync(Guid id, string key, [FromBody] SaveUserIdentifierPayload payload, CancellationToken cancellationToken)
  {
    UserModel? user = await _userService.SaveIdentifierAsync(id, key, payload, cancellationToken);
    return user == null ? NotFound() : Ok(user);
  }

  [HttpGet]
  public async Task<ActionResult<SearchResults<UserModel>>> SearchAsync([FromQuery] SearchUsersModel model, CancellationToken cancellationToken)
  {
    SearchUsersPayload payload = model.ToPayload();
    return Ok(await _userService.SearchAsync(payload, cancellationToken));
  }

  [HttpPut("{id}/sign/out")]
  public async Task<ActionResult<UserModel>> SignOutAsync(Guid id, CancellationToken cancellationToken)
  {
    UserModel? user = await _userService.SignOutAsync(id, cancellationToken);
    return user == null ? NotFound() : Ok(user);
  }

  [HttpPatch("{id}")]
  public async Task<ActionResult<UserModel>> UpdateAsync(Guid id, [FromBody] UpdateUserPayload payload, CancellationToken cancellationToken)
  {
    UserModel? user = await _userService.UpdateAsync(id, payload, cancellationToken);
    return user == null ? NotFound() : Ok(user);
  }

  private Uri BuildLocation(UserModel user) => HttpContext.BuildLocation("users/{id}", new Dictionary<string, string> { ["id"] = user.Id.ToString() });
}
