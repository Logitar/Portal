using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Portal.Core;
using Portal.Core.Users;
using Portal.Core.Users.Models;
using Portal.Core.Users.Payloads;

namespace Portal.Web.Controllers.Api
{
  [ApiController]
  [Authorize(Policy = Constants.Policies.PortalIdentity)]
  [Route("api/users")]
  public class UserApiController : ControllerBase
  {
    private readonly IUserService _userService;

    public UserApiController(IUserService userService)
    {
      _userService = userService;
    }

    [HttpPost]
    public async Task<ActionResult<UserModel>> CreateAsync([FromBody] CreateUserPayload payload, CancellationToken cancellationToken)
    {
      UserModel user = await _userService.CreateAsync(payload, cancellationToken);
      var uri = new Uri($"/api/users/{user.Id}", UriKind.Relative);

      return Created(uri, user);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<UserModel>> DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
      return Ok(await _userService.DeleteAsync(id, cancellationToken));
    }

    [HttpGet]
    public async Task<ActionResult<ListModel<UserModel>>> GetAsync(bool? isConfirmed, bool? isDisabled, Guid? realmId, string? search,
      UserSort? sort, bool desc,
      int? index, int? count,
      CancellationToken cancellationToken = default)
    {
      return Ok(await _userService.GetAsync(isConfirmed, isDisabled, realmId, search,
        sort, desc,
        index, count,
        cancellationToken));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<UserModel>> GetAsync(Guid id, CancellationToken cancellationToken)
    {
      UserModel? user = await _userService.GetAsync(id, cancellationToken);
      if (user == null)
      {
        return NotFound();
      }

      return Ok(user);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<UserModel>> UpdateAsync(Guid id, [FromBody] UpdateUserPayload payload, CancellationToken cancellationToken)
    {
      return Ok(await _userService.UpdateAsync(id, payload, cancellationToken));
    }

    [HttpPatch("{id}/disable")]
    public async Task<ActionResult<UserModel>> DisableAsync(Guid id, CancellationToken cancellationToken)
    {
      return Ok(await _userService.DisableAsync(id, cancellationToken));
    }

    [HttpPatch("{id}/enable")]
    public async Task<ActionResult<UserModel>> EnableAsync(Guid id, CancellationToken cancellationToken)
    {
      return Ok(await _userService.EnableAsync(id, cancellationToken));
    }
  }
}
