using Logitar.Portal.Application.Tokens.Commands;
using Logitar.Portal.Contracts.Tokens;

namespace Logitar.Portal.Application.Tokens;

internal class TokenFacade : ITokenService
{
  private readonly IActivityPipeline _activityPipeline;

  public TokenFacade(IActivityPipeline activityPipeline)
  {
    _activityPipeline = activityPipeline;
  }

  public async Task<CreatedToken> CreateAsync(CreateTokenPayload payload, CancellationToken cancellationToken)
  {
    return await _activityPipeline.ExecuteAsync(new CreateTokenCommand(payload), cancellationToken);
  }

  public async Task<ValidatedToken> ValidateAsync(ValidateTokenPayload payload, CancellationToken cancellationToken)
  {
    return await _activityPipeline.ExecuteAsync(new ValidateTokenCommand(payload), cancellationToken);
  }
}
