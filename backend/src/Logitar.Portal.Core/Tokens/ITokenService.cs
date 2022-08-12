using Logitar.Portal.Core.Tokens.Models;
using Logitar.Portal.Core.Tokens.Payloads;

namespace Logitar.Portal.Core.Tokens
{
  public interface ITokenService
  {
    Task<TokenModel> CreateAsync(CreateTokenPayload payload, CancellationToken cancellationToken = default);
    Task<ValidatedTokenModel> ValidateAsync(ValidateTokenPayload payload, bool consume = false, CancellationToken cancellationToken = default);
  }
}
