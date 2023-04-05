using Logitar.Portal.Core.Tokens.Models;
using Logitar.Portal.Core.Tokens.Payloads;

namespace Logitar.Portal.Client
{
  public interface ITokenService
  {
    Task<ValidatedTokenModel> ConsumeAsync(ValidateTokenPayload payload, CancellationToken cancellationToken = default);
    Task<TokenModel> CreateAsync(CreateTokenPayload payload, CancellationToken cancellationToken = default);
    Task<ValidatedTokenModel> ValidateAsync(ValidateTokenPayload payload, CancellationToken cancellationToken = default);
  }
}
