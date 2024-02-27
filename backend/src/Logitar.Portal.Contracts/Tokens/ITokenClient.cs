namespace Logitar.Portal.Contracts.Tokens;

public interface ITokenClient
{
  Task<CreatedToken> CreateAsync(CreateTokenPayload payload, IRequestContext? context = null);
  Task<ValidatedToken> ValidateAsync(ValidateTokenPayload payload, IRequestContext? context = null);
}
