using Logitar.Portal.Contracts.Tokens;

namespace Logitar.Portal.Application.Tokens
{
  public interface ITokenService
  {
    Task<TokenModel> CreateAsync(CreateTokenPayload payload, CancellationToken cancellationToken = default);
    Task<ValidatedTokenModel> ValidateAsync(ValidateTokenPayload payload, bool consume = false, CancellationToken cancellationToken = default);
  }
}
