using Logitar.Portal.Application.Tokens.Commands;
using Logitar.Portal.Contracts.Tokens;

namespace Logitar.Portal.Application.Tokens
{
  internal class TokenService : ITokenService
  {
    private readonly IRequestPipeline _requestPipeline;

    public TokenService(IRequestPipeline requestPipeline)
    {
      _requestPipeline = requestPipeline;
    }

    public async Task<TokenModel> CreateAsync(CreateTokenPayload payload, CancellationToken cancellationToken)
    {
      return await _requestPipeline.ExecuteAsync(new CreateTokenCommand(payload), cancellationToken);
    }

    public async Task<ValidatedTokenModel> ValidateAsync(ValidateTokenPayload payload, bool consume, CancellationToken cancellationToken)
    {
      return await _requestPipeline.ExecuteAsync(new ValidateTokenCommand(payload, consume), cancellationToken);
    }
  }
}
