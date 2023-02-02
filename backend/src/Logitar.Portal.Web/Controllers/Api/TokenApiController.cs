using Logitar.Portal.Application.Tokens;
using Logitar.Portal.Contracts.Tokens;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Logitar.Portal.Web.Controllers.Api
{
  [ApiController]
  [Authorize(Policy = Constants.Policies.PortalIdentity)]
  [Route("api/tokens")]
  public class TokenApiController : ControllerBase
  {
    private readonly ITokenService _tokenService;

    public TokenApiController(ITokenService tokenService)
    {
      _tokenService = tokenService;
    }

    [HttpPost("consume")]
    public async Task<ActionResult<ValidatedTokenModel>> ConsumeAsync([FromBody] ValidateTokenPayload payload, CancellationToken cancellationToken)
    {
      return Ok(await _tokenService.ValidateAsync(payload, consume: true, cancellationToken));
    }

    [HttpPost("create")]
    public async Task<ActionResult<TokenModel>> CreateAsync([FromBody] CreateTokenPayload payload, CancellationToken cancellationToken)
    {
      return Ok(await _tokenService.CreateAsync(payload, cancellationToken));
    }

    [HttpPost("validate")]
    public async Task<ActionResult<ValidatedTokenModel>> ValidateAsync([FromBody] ValidateTokenPayload payload, CancellationToken cancellationToken)
    {
      return Ok(await _tokenService.ValidateAsync(payload, consume: false, cancellationToken));
    }
  }
}
