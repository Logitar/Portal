using Logitar.Portal.Contracts.Constants;
using Logitar.Portal.Contracts.Tokens;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Logitar.Portal.Web.Controllers;

[ApiController]
[Authorize(Policy = Policies.PortalActor)]
[Route("api/tokens")]
public class TokenController : ControllerBase
{
  private readonly ITokenService _tokenService;

  public TokenController(ITokenService tokenService)
  {
    _tokenService = tokenService;
  }

  [HttpPost("create")]
  public async Task<ActionResult<CreatedToken>> CreateAsync([FromBody] CreateTokenPayload payload, CancellationToken cancellationToken)
  {
    return Ok(await _tokenService.CreateAsync(payload, cancellationToken));
  }

  [HttpPost("validate")]
  public async Task<ActionResult<ValidatedToken>> ValidateAsync([FromBody] ValidateTokenPayload payload, CancellationToken cancellationToken)
  {
    return Ok(await _tokenService.ValidateAsync(payload, cancellationToken));
  }
}
