namespace Logitar.Portal.Contracts.Tokens;

public interface ITokenClient
{
  Task<CreatedTokenModel> CreateAsync(CreateTokenPayload payload, IRequestContext? context = null);
  Task<ValidatedTokenModel> ValidateAsync(ValidateTokenPayload payload, IRequestContext? context = null);
}
