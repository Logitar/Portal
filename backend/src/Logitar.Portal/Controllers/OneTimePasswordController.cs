using Logitar.Portal.Contracts.Passwords;
using Logitar.Portal.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Logitar.Portal.Controllers;

[ApiController]
[Authorize]
[Route("one-time-passwords")]
public class OneTimePasswordController : ControllerBase
{
  private readonly IOneTimePasswordService _oneTimePasswordService;

  public OneTimePasswordController(IOneTimePasswordService oneTimePasswordService)
  {
    _oneTimePasswordService = oneTimePasswordService;
  }

  [HttpPost]
  public async Task<ActionResult<OneTimePassword>> CreateAsync([FromBody] CreateOneTimePasswordPayload payload, CancellationToken cancellationToken)
  {
    OneTimePassword oneTimePassword = await _oneTimePasswordService.CreateAsync(payload, cancellationToken);
    return Created(BuildLocation(oneTimePassword), oneTimePassword);
  }

  [HttpDelete("{id}")]
  public async Task<ActionResult<OneTimePassword>> DeleteAsync(Guid id, CancellationToken cancellationToken)
  {
    OneTimePassword? oneTimePassword = await _oneTimePasswordService.DeleteAsync(id, cancellationToken);
    return oneTimePassword == null ? NotFound() : Ok(oneTimePassword);
  }

  [HttpGet("{id}")]
  public async Task<ActionResult<OneTimePassword>> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    OneTimePassword? oneTimePassword = await _oneTimePasswordService.ReadAsync(id: id, cancellationToken: cancellationToken);
    return oneTimePassword == null ? NotFound() : Ok(oneTimePassword);
  }

  private Uri BuildLocation(OneTimePassword oneTimePassword) => HttpContext.BuildLocation("one-time-passwords/{id}", new Dictionary<string, string> { ["id"] = oneTimePassword.Id.ToString() });
}
