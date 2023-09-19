using Logitar.Portal.Application.Tokens.Commands;
using Logitar.Portal.Contracts.Tokens;

namespace Logitar.Portal.Application.Tokens;

internal class TokenService : ITokenService
{
  private readonly IRequestPipeline _pipeline;

  public TokenService(IRequestPipeline pipeline)
  {
    _pipeline = pipeline;
  }

  public async Task<CreatedToken> CreateAsync(CreateTokenPayload payload, CancellationToken cancellationToken)
  {
    return await _pipeline.ExecuteAsync(new CreateTokenCommand(payload), cancellationToken);
  }

  public async Task<ValidatedToken> ValidateAsync(ValidateTokenPayload payload, CancellationToken cancellationToken)
  {
    return await _pipeline.ExecuteAsync(new ValidateTokenCommand(payload), cancellationToken);
  }
}
