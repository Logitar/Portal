using Logitar.Portal.Contracts.Tokens;

namespace Logitar.Portal.Application.Tokens;

public interface ITokenService
{
  Task<CreatedTokenModel> CreateAsync(CreateTokenPayload payload, CancellationToken cancellationToken = default);
  Task<ValidatedTokenModel> ValidateAsync(ValidateTokenPayload payload, CancellationToken cancellationToken = default);
}
