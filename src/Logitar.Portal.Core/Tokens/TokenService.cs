using Logitar.Portal.Contracts.Tokens;
using Logitar.Portal.Core.Tokens.Commands;

namespace Logitar.Portal.Core.Tokens;

internal class TokenService : ITokenService
{
  private readonly IRequestPipeline _pipeline;

  public TokenService(IRequestPipeline pipeline)
  {
    _pipeline = pipeline;
  }

  public async Task<ValidatedToken> ConsumeAsync(ValidateTokenInput input, CancellationToken cancellationToken)
  {
    return await _pipeline.ExecuteAsync(new ValidateToken(input, Consume: true), cancellationToken);
  }

  public async Task<CreatedToken> CreateAsync(CreateTokenInput input, CancellationToken cancellationToken)
  {
    return await _pipeline.ExecuteAsync(new CreateToken(input), cancellationToken);
  }

  public async Task<ValidatedToken> ValidateAsync(ValidateTokenInput input, CancellationToken cancellationToken)
  {
    return await _pipeline.ExecuteAsync(new ValidateToken(input, Consume: false), cancellationToken);
  }
}
