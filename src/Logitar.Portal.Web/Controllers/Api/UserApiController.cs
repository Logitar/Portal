using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Messages;
using Logitar.Portal.Contracts.Users;
using Logitar.Portal.Core.Realms;
using Logitar.Portal.Web.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Logitar.Portal.Web.Controllers.Api;

[ApiController]
[Authorize(Policy = Policies.PortalActor)]
[Route("api/users")]
public class UserApiController : ControllerBase
{
  private readonly IUserService _userService;

  public UserApiController(IUserService userService)
  {
    _userService = userService;
  }

  [HttpPost]
  public async Task<ActionResult<User>> CreateAsync([FromBody] CreateUserInput input, CancellationToken cancellationToken)
  {
    User user = await _userService.CreateAsync(input, cancellationToken);
    Uri uri = new($"{Request.Scheme}://{Request.Host}/api/users/{user.Id}");

    return Created(uri, user);
  }

  [HttpDelete("{id}")]
  public async Task<ActionResult<User>> DeleteAsync(Guid id, CancellationToken cancellationToken)
  {
    return Ok(await _userService.DeleteAsync(id, cancellationToken));
  }

  [HttpGet]
  public async Task<ActionResult<PagedList<User>>> GetAsync(bool? isConfirmed, bool? isDisabled, string? realm, string? search,
    UserSort? sort, bool isDescending, int? skip, int? limit, CancellationToken cancellationToken)
  {
    return Ok(await _userService.GetAsync(isConfirmed, isDisabled, realm, search,
      sort, isDescending, skip, limit, cancellationToken));
  }

  [HttpGet("{id}")]
  public async Task<ActionResult<User>> GetAsync(Guid id, CancellationToken cancellationToken)
  {
    User? user = await _userService.GetAsync(id, cancellationToken: cancellationToken);
    if (user == null)
    {
      return NotFound();
    }

    return Ok(user);
  }

  [HttpPut("{id}")]
  public async Task<ActionResult<User>> UpdateAsync(Guid id, [FromBody] UpdateUserInput input, CancellationToken cancellationToken)
  {
    return Ok(await _userService.UpdateAsync(id, input, cancellationToken));
  }

  [HttpPatch("{id}/password/change")]
  public async Task<ActionResult<User>> ChangePasswordAsync(Guid id, [FromBody] ChangePasswordInput input, CancellationToken cancellationToken)
  {
    return Ok(await _userService.ChangePasswordAsync(id, input, cancellationToken));
  }

  [HttpPost("password/recover")]
  public async Task<ActionResult<SentMessages>> RecoverPasswordAsync([FromBody] RecoverPasswordInput input, CancellationToken cancellationToken)
  {
    if (input.Realm == RealmAggregate.PortalUniqueName)
    {
      return Forbid();
    }

    return Ok(await _userService.RecoverPasswordAsync(input, cancellationToken));
  }

  [HttpPatch("password/reset")]
  public async Task<ActionResult<User>> ResetPasswordAsync([FromBody] ResetPasswordInput input, CancellationToken cancellationToken)
  {
    if (input.Realm == RealmAggregate.PortalUniqueName)
    {
      return Forbid();
    }

    return Ok(await _userService.ResetPasswordAsync(input, cancellationToken));
  }

  [HttpPatch("{id}/disable")]
  public async Task<ActionResult<User>> DisableAsync(Guid id, CancellationToken cancellationToken)
  {
    return Ok(await _userService.DisableAsync(id, cancellationToken));
  }

  [HttpPatch("{id}/enable")]
  public async Task<ActionResult<User>> EnableAsync(Guid id, CancellationToken cancellationToken)
  {
    return Ok(await _userService.EnableAsync(id, cancellationToken));
  }

  [HttpPatch("{id}/external-identifiers/{key}")]
  public async Task<ActionResult<User>> SetExternalIdentifierAsync(Guid id, string key, string? value, CancellationToken cancellationToken)
  {
    return Ok(await _userService.SetExternalIdentifierAsync(id, key, value, cancellationToken));
  }

  [HttpPatch("{id}/address/verify")]
  public async Task<ActionResult<User>> VerifyAddressAsync(Guid id, CancellationToken cancellationToken)
  {
    return Ok(await _userService.VerifyAddressAsync(id, cancellationToken));
  }

  [HttpPatch("{id}/email/verify")]
  public async Task<ActionResult<User>> VerifyEmailAsync(Guid id, CancellationToken cancellationToken)
  {
    return Ok(await _userService.VerifyEmailAsync(id, cancellationToken));
  }

  [HttpPatch("{id}/phone/verify")]
  public async Task<ActionResult<User>> VerifyPhoneAsync(Guid id, CancellationToken cancellationToken)
  {
    return Ok(await _userService.VerifyPhoneAsync(id, cancellationToken));
  }
}
