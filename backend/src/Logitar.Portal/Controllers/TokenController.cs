using Logitar.Portal.Contracts.Tokens;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Logitar.Portal.Controllers;

[ApiController]
[Authorize]
[Route("tokens")]
public class TokenController : ControllerBase
{
  private readonly ITokenService _tokenService;

  public TokenController(ITokenService tokenService)
  {
    _tokenService = tokenService;
  }

  [HttpPost]
  public async Task<ActionResult<CreatedToken>> CreateAsync([FromBody] CreateTokenPayload payload, CancellationToken cancellationToken)
  {
    return Ok(await _tokenService.CreateAsync(payload, cancellationToken)); // TODO(fpion): Created with validation URL
  }

  //private Uri BuildLocation(User user) => HttpContext.BuildLocation("users/{id}", new Dictionary<string, string> { ["id"] = user.Id.ToString() });
}
