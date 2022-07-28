using Portal.Core.Tokens.Models;
using Portal.Core.Tokens.Payloads;

namespace Portal.Core.Tokens
{
  public interface ITokenService
  {
    Task<TokenModel> CreateAsync(CreateTokenPayload payload, CancellationToken cancellationToken = default);
    Task<ValidatedTokenModel> ValidateAsync(ValidateTokenPayload payload, bool consume = false, CancellationToken cancellationToken = default);
  }
}
