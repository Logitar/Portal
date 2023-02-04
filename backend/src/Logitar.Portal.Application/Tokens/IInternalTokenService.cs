using Logitar.Portal.Contracts.Tokens;

namespace Logitar.Portal.Application.Tokens
{
  internal interface IInternalTokenService
  {
    Task<TokenModel> CreateAsync(CreateTokenPayload payload, CancellationToken cancellationToken = default);
    Task<ValidatedTokenModel> ValidateAsync(ValidateTokenPayload payload, bool consume = false, CancellationToken cancellationToken = default);
  }
}
