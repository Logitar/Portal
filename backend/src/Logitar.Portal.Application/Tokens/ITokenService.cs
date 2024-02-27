using Logitar.Portal.Contracts.Tokens;

namespace Logitar.Portal.Application.Tokens;

public interface ITokenService
{
  Task<CreatedToken> CreateAsync(CreateTokenPayload payload, CancellationToken cancellationToken = default);
  Task<ValidatedToken> ValidateAsync(ValidateTokenPayload payload, CancellationToken cancellationToken = default);
}
