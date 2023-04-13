namespace Logitar.Portal.Contracts.Tokens;

public interface ITokenService
{
  Task<CreatedToken> CreateAsync(CreateTokenInput input, CancellationToken cancellationToken = default);
  Task<ValidatedToken> ValidateAsync(ValidateTokenInput input, bool consume = false, CancellationToken cancellationToken = default);
}
