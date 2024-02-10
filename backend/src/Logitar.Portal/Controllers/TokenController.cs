using Logitar.Portal.Contracts.Tokens;
using Logitar.Portal.Extensions;
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
    CreatedToken createdToken = await _tokenService.CreateAsync(payload, cancellationToken);
    return Created(BuildLocation(), createdToken);
  }

  [HttpPut]
  public async Task<ActionResult<ValidatedToken>> ValidateAsync([FromBody] ValidateTokenPayload payload, CancellationToken cancellationToken)
  {
    return Ok(await _tokenService.ValidateAsync(payload, cancellationToken));
  }

  private Uri BuildLocation() => HttpContext.BuildLocation("tokens");
}
