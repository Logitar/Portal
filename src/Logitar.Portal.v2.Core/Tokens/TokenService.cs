using Logitar.Portal.v2.Contracts.Tokens;
using Logitar.Portal.v2.Core.Tokens.Commands;

namespace Logitar.Portal.v2.Core.Tokens;

internal class TokenService : ITokenService
{
  private readonly IRequestPipeline _pipeline;

  public TokenService(IRequestPipeline pipeline)
  {
    _pipeline = pipeline;
  }

  public async Task<CreatedToken> CreateAsync(CreateTokenInput input, CancellationToken cancellationToken)
  {
    return await _pipeline.ExecuteAsync(new CreateToken(input), cancellationToken);
  }

  public async Task<ValidatedToken> ValidateAsync(ValidateTokenInput input, bool consume, CancellationToken cancellationToken)
  {
    return await _pipeline.ExecuteAsync(new ValidateToken(input, consume), cancellationToken);
  }
}
