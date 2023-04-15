using Logitar.Portal.Contracts.Tokens;
using Logitar.Portal.Web.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Logitar.Portal.Web.Controllers.Api;

[ApiController]
[Authorize(Policy = Policies.PortalActor)]
[Route("api/tokens")]
public class TokenApiController : ControllerBase
{
  private readonly ITokenService _tokenService;

  public TokenApiController(ITokenService tokenService)
  {
    _tokenService = tokenService;
  }

  [HttpPost("consume")]
  public async Task<ActionResult<ValidatedToken>> ConsumeAsync([FromBody] ValidateTokenInput input, CancellationToken cancellationToken)
  {
    return Ok(await _tokenService.ConsumeAsync(input, cancellationToken));
  }

  [HttpPost("create")]
  public async Task<ActionResult<CreatedToken>> CreateAsync([FromBody] CreateTokenInput input, CancellationToken cancellationToken)
  {
    return Ok(await _tokenService.CreateAsync(input, cancellationToken));
  }

  [HttpPost("validate")]
  public async Task<ActionResult<ValidatedToken>> ValidateAsync([FromBody] ValidateTokenInput input, CancellationToken cancellationToken)
  {
    return Ok(await _tokenService.ValidateAsync(input, cancellationToken));
  }
}
