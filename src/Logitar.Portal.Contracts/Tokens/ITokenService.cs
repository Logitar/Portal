namespace Logitar.Portal.Contracts.Tokens;

public interface ITokenService
{
  Task<ValidatedToken> ConsumeAsync(ValidateTokenInput input, CancellationToken cancellationToken = default);
  Task<CreatedToken> CreateAsync(CreateTokenInput input, CancellationToken cancellationToken = default);
  Task<ValidatedToken> ValidateAsync(ValidateTokenInput input, CancellationToken cancellationToken = default);
}
