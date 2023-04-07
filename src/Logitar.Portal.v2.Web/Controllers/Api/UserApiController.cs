using Logitar.Portal.v2.Contracts;
using Logitar.Portal.v2.Contracts.Users;
using Microsoft.AspNetCore.Mvc;

namespace Logitar.Portal.v2.Web.Controllers.Api;

[ApiController]
//[Authorize(Policy = Constants.Policies.PortalIdentity)] // TODO(fpion): Authorization
[Route("api/users")]
public class UserApiController : ControllerBase
{
  /*
  Task<User> SetExternalIdentifierAsync(Guid id, string key, string? value, CancellationToken cancellationToken = default);
   */

  private readonly IUserService _userService;

  public UserApiController(IUserService userService)
  {
    _userService = userService;
  }

  [HttpPost]
  public async Task<ActionResult<User>> CreateAsync([FromBody] CreateUserInput input, CancellationToken cancellationToken)
  {
    User user = await _userService.CreateAsync(input, cancellationToken);
    var uri = new Uri($"/api/users/{user.Id}", UriKind.Relative);

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
