namespace Logitar.Portal.v2.Contracts.Tokens;

public interface ITokenService
{
  Task<CreatedToken> CreateAsync(CreateTokenInput input, CancellationToken cancellationToken = default);
  Task<ValidatedToken> ValidateAsync(ValidateTokenInput input, bool consume = false, CancellationToken cancellationToken = default);
}
